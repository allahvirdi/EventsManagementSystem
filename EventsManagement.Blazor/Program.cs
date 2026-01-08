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
builder.Services.AddScoped<AuthHttpHandler>();

// تنظیم HttpClient با AuthHttpHandler
builder.Services.AddHttpClient("API", client => 
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<AuthHttpHandler>();

// HttpClient پیش‌فرض برای سایر موارد
builder.Services.AddScoped(sp => 
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    return httpClientFactory.CreateClient("API");
});

await builder.Build().RunAsync();
