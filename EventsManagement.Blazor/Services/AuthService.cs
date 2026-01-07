using System.Net.Http.Json;
using System.Text.Json;

namespace EventsManagement.Blazor.Services
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(string email, string password);
        Task LogoutAsync();
        Task<string?> GetTokenAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<UserInfo?> GetUserInfoAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private const string TokenKey = "authToken";
        private const string RefreshTokenKey = "refreshToken";
        private const string UserInfoKey = "userInfo";

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Auth/Login", new
                {
                    email,
                    password
                });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<LoginData>>();
                    
                    if (result?.IsSuccess == true && result.Data != null)
                    {
                        // ذخیره Token در localStorage
                        await SaveTokenAsync(result.Data.AccessToken, result.Data.RefreshToken);
                        
                        // ذخیره اطلاعات کاربر
                        var userInfo = new UserInfo
                        {
                            UserId = result.Data.UserId,
                            FullName = result.Data.FullName,
                            Email = result.Data.Email,
                            Roles = result.Data.Roles
                        };
                        await SaveUserInfoAsync(userInfo);

                        return new LoginResult { Success = true, Message = result.Message };
                    }

                    return new LoginResult { Success = false, Message = result?.Message ?? "خطای ناشناخته" };
                }

                return new LoginResult { Success = false, Message = "خطا در ارتباط با سرور" };
            }
            catch (Exception ex)
            {
                return new LoginResult { Success = false, Message = $"خطا: {ex.Message}" };
            }
        }

        public async Task LogoutAsync()
        {
            await RemoveTokenAsync();
            await RemoveUserInfoAsync();
        }

        public async Task<string?> GetTokenAsync()
        {
            // در Blazor WebAssembly از localStorage استفاده می‌کنیم
            // این متد باید با JSInterop پیاده‌سازی شود
            return await Task.FromResult<string?>(null);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public async Task<UserInfo?> GetUserInfoAsync()
        {
            // این متد باید با JSInterop پیاده‌سازی شود
            return await Task.FromResult<UserInfo?>(null);
        }

        private async Task SaveTokenAsync(string accessToken, string refreshToken)
        {
            // TODO: پیاده‌سازی با JSInterop
            await Task.CompletedTask;
        }

        private async Task SaveUserInfoAsync(UserInfo userInfo)
        {
            // TODO: پیاده‌سازی با JSInterop
            await Task.CompletedTask;
        }

        private async Task RemoveTokenAsync()
        {
            // TODO: پیاده‌سازی با JSInterop
            await Task.CompletedTask;
        }

        private async Task RemoveUserInfoAsync()
        {
            // TODO: پیاده‌سازی با JSInterop
            await Task.CompletedTask;
        }
    }

    // DTOs
    public class LoginResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class UserInfo
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }

    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }

    public class LoginData
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
