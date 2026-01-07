using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using EventsManagement.Shared.Entities;
using EventsManagement.Shared.DTOs;
using EventsManagement.API.Services;
using EventsManagement.Infrastructure.Data;
using Serilog;

namespace EventsManagement.API.Controllers
{
    /// <summary>
    /// کنترلر احراز هویت
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ApplicationDbContext _context;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IJwtTokenService jwtTokenService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _context = context;
        }

        /// <summary>
        /// ورود کاربر
        /// POST: api/Auth/Login
        /// </summary>
        [HttpPost("Login")]
        public async Task<ActionResult<ApiResult<LoginResponseDto>>> Login([FromBody] LoginDto model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    Log.Warning($"تلاش ناموفق ورود: کاربر {model.Username} یافت نشد");
                    return Ok(ApiResult<LoginResponseDto>.Failure("نام کاربری یا رمز عبور اشتباه است", 401));
                }

                if (!user.IsActive)
                {
                    Log.Warning($"تلاش ناموفق ورود: کاربر {model.Username} غیرفعال است");
                    return Ok(ApiResult<LoginResponseDto>.Failure("حساب کاربری غیرفعال است", 403));
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (!result.Succeeded)
                {
                    Log.Warning($"تلاش ناموفق ورود: رمز عبور اشتباه برای {model.Username}");
                    return Ok(ApiResult<LoginResponseDto>.Failure("نام کاربری یا رمز عبور اشتباه است", 401));
                }

                // تولید توکن
                var tokenResponse = await _jwtTokenService.GenerateTokenAsync(user);

                // به‌روزرسانی آخرین زمان ورود
                user.LastLoginDate = DateTime.Now;
                await _userManager.UpdateAsync(user);

                // ثبت لاگ ورود
                await LogUserActivity(user.Id, "Login", "ورود موفق به سامانه", true);

                Log.Information($"ورود موفق کاربر: {user.UserName}");

                return Ok(ApiResult<LoginResponseDto>.Success(tokenResponse, "ورود موفقیت‌آمیز"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در ورود کاربر");
                return Ok(ApiResult<LoginResponseDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// خروج کاربر
        /// POST: api/Auth/Logout
        /// </summary>
        [HttpPost("Logout")]
        [Authorize]
        public async Task<ActionResult<ApiResult>> Logout()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                    return Ok(ApiResult.Failure("کاربر یافت نشد", 401));

                // ابطال توکن‌های کاربر
                await _jwtTokenService.RevokeTokenAsync(userId);

                // ثبت لاگ خروج
                await LogUserActivity(userId, "Logout", "خروج از سامانه", true);

                Log.Information($"خروج موفق کاربر: {userId}");

                return Ok(ApiResult.Success("خروج موفقیت‌آمیز"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در خروج کاربر");
                return Ok(ApiResult.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// تازه‌سازی توکن
        /// POST: api/Auth/RefreshToken
        /// </summary>
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<ApiResult<LoginResponseDto>>> RefreshToken([FromBody] RefreshTokenDto model)
        {
            try
            {
                var tokenResponse = await _jwtTokenService.RefreshTokenAsync(model.RefreshToken);
                Log.Information($"تازه‌سازی توکن موفق برای کاربر: {tokenResponse.UserId}");
                return Ok(ApiResult<LoginResponseDto>.Success(tokenResponse, "توکن با موفقیت تازه‌سازی شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در تازه‌سازی توکن");
                return Ok(ApiResult<LoginResponseDto>.Failure("توکن نامعتبر است", 401));
            }
        }

        /// <summary>
        /// تغییر رمز عبور
        /// POST: api/Auth/ChangePassword
        /// </summary>
        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<ActionResult<ApiResult>> ChangePassword([FromBody] ChangePasswordDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                    return Ok(ApiResult.Failure("کاربر یافت نشد", 404));

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return Ok(ApiResult.Failure("تغییر رمز عبور ناموفق بود", 400, 
                        new Dictionary<string, string[]> { { "Password", errors.ToArray() } }));
                }

                // ثبت لاگ تغییر رمز
                await LogUserActivity(userId, "ChangePassword", "تغییر رمز عبور", true);

                Log.Information($"تغییر رمز عبور موفق برای کاربر: {user.UserName}");

                return Ok(ApiResult.Success("رمز عبور با موفقیت تغییر یافت"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در تغییر رمز عبور");
                return Ok(ApiResult.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت اطلاعات کاربر جاری
        /// GET: api/Auth/Me
        /// </summary>
        [HttpGet("Me")]
        [Authorize]
        public async Task<ActionResult<ApiResult<LoginResponseDto>>> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                    return Ok(ApiResult<LoginResponseDto>.Failure("کاربر یافت نشد", 404));

                var roles = await _userManager.GetRolesAsync(user);

                var userInfo = new LoginResponseDto
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Username = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList()
                };

                return Ok(ApiResult<LoginResponseDto>.Success(userInfo));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در دریافت اطلاعات کاربر");
                return Ok(ApiResult<LoginResponseDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ثبت فعالیت کاربر
        /// </summary>
        private async Task LogUserActivity(string userId, string activityType, string description, bool isSuccessful)
        {
            var log = new UserActivityLog
            {
                UserId = userId,
                ActivityType = activityType,
                Description = description,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                IsSuccessful = isSuccessful,
                ActivityDateTime = DateTime.Now,
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            };

            _context.UserActivityLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
