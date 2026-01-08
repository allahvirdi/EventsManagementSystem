using System.Net.Http.Json;
using System.Text.Json;

namespace EventsManagement.Blazor.Services
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(string username, string password);
        Task LogoutAsync();
        Task<string?> GetTokenAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<UserInfo?> GetUserInfoAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private const string TokenKey = "authToken";
        private const string RefreshTokenKey = "refreshToken";
        private const string UserInfoKey = "userInfo";

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Auth/Login", new
                {
                    username,
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
            return await _localStorage.GetItemAsync<string>(TokenKey);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public async Task<UserInfo?> GetUserInfoAsync()
        {
            return await _localStorage.GetItemAsync<UserInfo>(UserInfoKey);
        }

        private async Task SaveTokenAsync(string accessToken, string refreshToken)
        {
            await _localStorage.SetItemAsync(TokenKey, accessToken);
            await _localStorage.SetItemAsync(RefreshTokenKey, refreshToken);
        }

        private async Task SaveUserInfoAsync(UserInfo userInfo)
        {
            await _localStorage.SetItemAsync(UserInfoKey, userInfo);
        }

        private async Task RemoveTokenAsync()
        {
            await _localStorage.RemoveItemAsync(TokenKey);
            await _localStorage.RemoveItemAsync(RefreshTokenKey);
        }

        private async Task RemoveUserInfoAsync()
        {
            await _localStorage.RemoveItemAsync(UserInfoKey);
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
