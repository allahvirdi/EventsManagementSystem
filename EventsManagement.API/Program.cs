using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using FluentValidation;
using MediatR;
using EventsManagement.Infrastructure.Data;
using EventsManagement.Shared.Entities;
using EventsManagement.Infrastructure.Repositories;

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

// ============= MediatR کانفیگ =============
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(EventsManagement.Application.Validators.AuthValidators).Assembly));

// ============= FluentValidation کانفیگ =============
builder.Services.AddValidatorsFromAssemblyContaining(typeof(EventsManagement.Application.Validators.LoginValidator));
builder.Services.AddFluentValidationClientsideAdapters();

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

// ============= Database Migration =============
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
    
    // Seeding Admin User
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
    // ایجاد نقش‌ها
    string[] roleNames = { "Admin", "Manager", "Operator", "Viewer" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
    
    // ایجاد کاربر Admin
    var adminUser = await userManager.FindByNameAsync("admin");
    if (adminUser == null)
    {
        var newAdmin = new AppUser
        {
            UserName = "admin",
            Email = "admin@example.com",
            FullName = "مدیر سامانه",
            EmailConfirmed = true,
            IsActive = true
        };
        
        var result = await userManager.CreateAsync(newAdmin, "!QAZ1qaz");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }
}

Log.Information("EventsManagement API است برای شروع");
app.Run();
