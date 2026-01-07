using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EventsManagement.Shared.Entities;
using EventsManagement.Shared.DTOs;
using EventsManagement.Infrastructure.Data;
using Serilog;

namespace EventsManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UsersController(UserManager<AppUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        /// دریافت لیست کاربران
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<UserDto>>>> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? role = null)
        {
            try
            {
                var query = _userManager.Users.AsQueryable();

                // فیلتر بر اساس نقش
                if (!string.IsNullOrEmpty(role))
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                    var userIds = usersInRole.Select(u => u.Id).ToList();
                    query = query.Where(u => userIds.Contains(u.Id));
                }

                var totalCount = await query.CountAsync();
                var users = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var userDtos = new List<UserDto>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userDtos.Add(new UserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName ?? "",
                        Email = user.Email ?? "",
                        FullName = user.FullName,
                        PhoneNumber = user.PhoneNumber,
                        IsActive = user.IsActive,
                        Roles = roles.ToList(),
                        CreatedAt = user.CreatedAt
                    });
                }

                return Ok(ApiResult<List<UserDto>>.Success(userDtos, $"تعداد {totalCount} کاربر یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در دریافت لیست کاربران");
                return Ok(ApiResult<List<UserDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ایجاد کاربر جدید
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResult<string>>> CreateUser([FromBody] CreateUserDto model)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                    return Ok(ApiResult<string>.Failure("کاربری با این ایمیل قبلاً ثبت شده است", 400));

                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return Ok(ApiResult<string>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)), 400));

                // اضافه کردن نقش‌ها
                if (model.Roles != null && model.Roles.Any())
                {
                    await _userManager.AddToRolesAsync(user, model.Roles);
                }

                Log.Information($"کاربر جدید ایجاد شد: {user.Email}");
                return Ok(ApiResult<string>.Success(user.Id, "کاربر با موفقیت ایجاد شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در ایجاد کاربر");
                return Ok(ApiResult<string>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ویرایش کاربر
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult>> UpdateUser(string id, [FromBody] UpdateUserDto model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return Ok(ApiResult.Failure("کاربر یافت نشد", 404));

                user.FullName = model.FullName;
                user.PhoneNumber = model.PhoneNumber;
                user.IsActive = model.IsActive;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return Ok(ApiResult.Failure(string.Join(", ", result.Errors.Select(e => e.Description)), 400));

                // به‌روزرسانی نقش‌ها
                if (model.Roles != null)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    await _userManager.AddToRolesAsync(user, model.Roles);
                }

                Log.Information($"کاربر به‌روزرسانی شد: {user.Email}");
                return Ok(ApiResult.Success("کاربر با موفقیت به‌روزرسانی شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در به‌روزرسانی کاربر {id}");
                return Ok(ApiResult.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ریست رمز عبور کاربر
        /// </summary>
        [HttpPost("{id}/reset-password")]
        public async Task<ActionResult<ApiResult>> ResetPassword(string id, [FromBody] ResetPasswordDto model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return Ok(ApiResult.Failure("کاربر یافت نشد", 404));

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (!result.Succeeded)
                    return Ok(ApiResult.Failure(string.Join(", ", result.Errors.Select(e => e.Description)), 400));

                Log.Information($"رمز عبور کاربر ریست شد: {user.Email}");
                return Ok(ApiResult.Success("رمز عبور با موفقیت تغییر یافت"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در ریست رمز کاربر {id}");
                return Ok(ApiResult.Failure("خطای سیستمی رخ داده است", 500));
            }
        }
    }

    // DTOs
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    public class CreateUserDto
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class UpdateUserDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public List<string>? Roles { get; set; }
    }

    public class ResetPasswordDto
    {
        public string NewPassword { get; set; } = string.Empty;
    }
}
