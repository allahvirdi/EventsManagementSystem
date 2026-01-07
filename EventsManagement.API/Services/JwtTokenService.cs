using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventsManagement.Shared.Entities;
using EventsManagement.Shared.DTOs;

namespace EventsManagement.API.Services
{
    /// <summary>
    /// سرویس تولید و مدیریت JWT Token
    /// </summary>
    public interface IJwtTokenService
    {
        Task<LoginResponseDto> GenerateTokenAsync(AppUser user);
        Task<LoginResponseDto> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string userId);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private static readonly Dictionary<string, string> _refreshTokens = new();

        public JwtTokenService(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        /// <summary>
        /// تولید Access Token و Refresh Token
        /// </summary>
        public async Task<LoginResponseDto> GenerateTokenAsync(AppUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expirationMinutes = int.Parse(jwtSettings["AccessTokenExpirationMinutes"]);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("FullName", user.FullName ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // اضافه کردن نقش‌ها
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = Guid.NewGuid().ToString();

            // ذخیره Refresh Token
            _refreshTokens[refreshToken] = user.Id;

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = expirationMinutes * 60,
                UserId = user.Id,
                FullName = user.FullName,
                Username = user.UserName,
                Email = user.Email,
                Roles = roles.ToList()
            };
        }

        /// <summary>
        /// تازه‌سازی توکن با Refresh Token
        /// </summary>
        public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
        {
            if (!_refreshTokens.ContainsKey(refreshToken))
                throw new Exception("Refresh Token نامعتبر است");

            var userId = _refreshTokens[refreshToken];
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || !user.IsActive)
                throw new Exception("کاربر یافت نشد یا غیرفعال است");

            // حذف توکن قدیمی
            _refreshTokens.Remove(refreshToken);

            // تولید توکن جدید
            return await GenerateTokenAsync(user);
        }

        /// <summary>
        /// ابطال توکن کاربر
        /// </summary>
        public Task<bool> RevokeTokenAsync(string userId)
        {
            var tokensToRemove = _refreshTokens.Where(x => x.Value == userId).Select(x => x.Key).ToList();
            foreach (var token in tokensToRemove)
            {
                _refreshTokens.Remove(token);
            }
            return Task.FromResult(true);
        }
    }
}
