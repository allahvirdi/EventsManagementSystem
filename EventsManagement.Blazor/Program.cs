using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using EventsManagement.Blazor;
using EventsManagement.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// خواندن API URL از appsettings
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7026";

// تنظیم HttpClient برای API
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(apiBaseUrl)
});

// ثبت سرویس‌ها
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<IAuthService, AuthService>();

await builder.Build().RunAsync();
