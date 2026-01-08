using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using EventsManagement.Blazor;
using EventsManagement.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// خواندن API URL از appsettings
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7026";

// ثبت سرویس‌ها
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ثبت AuthHttpHandler برای مدیریت خطاهای 401
builder.Services.AddTransient<AuthHttpHandler>();

// تنظیم HttpClient پیش‌فرض با BaseAddress
builder.Services.AddScoped(sp => 
{
    var handler = sp.GetRequiredService<AuthHttpHandler>();
    handler.InnerHandler = new HttpClientHandler();
    
    var client = new HttpClient(handler)
    {
        BaseAddress = new Uri(apiBaseUrl)
    };
    return client;
});

await builder.Build().RunAsync();
