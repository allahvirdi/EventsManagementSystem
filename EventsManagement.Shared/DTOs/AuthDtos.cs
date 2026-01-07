namespace EventsManagement.Shared.DTOs
{
    /// <summary>
    /// DTO برای احراز هویت و ورود
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// نام کاربری
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// رمز عبور
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// DTO برای پاسخ ورود
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// توکن دسترسی
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// توکن تازه‌سازی
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// مدت زمان انقضای توکن به ثانیه
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// نام کامل کاربر
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// نام کاربری
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// ایمیل
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// نقش‌های کاربر
        /// </summary>
        public List<string> Roles { get; set; } = new();
    }

    /// <summary>
    /// DTO برای تازه‌سازی توکن
    /// </summary>
    public class RefreshTokenDto
    {
        /// <summary>
        /// توکن تازه‌سازی
        /// </summary>
        public string RefreshToken { get; set; }
    }

    /// <summary>
    /// DTO برای تغییر رمز عبور
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// رمز عبور فعلی
        /// </summary>
        public string CurrentPassword { get; set; }

        /// <summary>
        /// رمز عبور جدید
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// تائید رمز عبور جدید
        /// </summary>
        public string ConfirmPassword { get; set; }
    }
}
