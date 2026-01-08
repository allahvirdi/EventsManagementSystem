using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using FluentValidation;
using EventsManagement.Infrastructure.Data;
using EventsManagement.Shared.Entities;
using EventsManagement.Infrastructure.Repositories;
using EventsManagement.API.Services;
using EventsManagement.Application.Validators;

var builder = WebApplication.CreateBuilder(args);

// ============= Serilog کانفیگ =============
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "EventsManagement.API")
    .CreateLogger();

builder.Host.UseSerilog();

// ============= DbContext کانفیگ =============
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(local);Database=EventsManagementDB;Integrated Security=true;";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ============= Identity کانفیگ =============
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ============= JWT کانفیگ =============
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey))
    };
});

// ============= Repository کانفیگ =============
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskReplyRepository, TaskReplyRepository>();
builder.Services.AddScoped<IDynamicTableRepository, DynamicTableRepository>();
builder.Services.AddScoped<IOrganizationUnitRepository, OrganizationUnitRepository>();

// ============= Services =============
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

// ============= MediatR کانفیگ =============
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(LoginValidator).Assembly); // Application
    cfg.RegisterServicesFromAssembly(typeof(ApplicationDbContext).Assembly); // Infrastructure
});

// ============= FluentValidation کانفیگ =============
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();

// ============= AutoMapper کانفیگ =============
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ============= Controllers و CORS =============
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// ============= OpenAPI کانفیگ =============
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ============= Middleware Pipeline =============
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ============= Database Migration & Seeding =============
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
    
    // Data Seeding
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataSeeder>>();
    
    var seeder = new DataSeeder(dbContext, userManager, roleManager, logger);
    await seeder.SeedAsync();
}

Log.Information("EventsManagement API است برای شروع");
app.Run();
