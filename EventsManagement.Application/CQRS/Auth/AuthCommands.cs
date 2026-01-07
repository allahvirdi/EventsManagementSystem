using MediatR;
using EventsManagement.Shared.DTOs;

namespace EventsManagement.Application.CQRS.Auth
{
    /// <summary>
    /// Command برای ورود کاربر
    /// </summary>
    public class LoginCommand : IRequest<LoginResponseDto>
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginCommand(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    /// <summary>
    /// Command برای خروج کاربر
    /// </summary>
    public class LogoutCommand : IRequest<bool>
    {
        public string UserId { get; set; }

        public LogoutCommand(string userId)
        {
            UserId = userId;
        }
    }

    /// <summary>
    /// Command برای تازه‌سازی توکن
    /// </summary>
    public class RefreshTokenCommand : IRequest<LoginResponseDto>
    {
        public string RefreshToken { get; set; }

        public RefreshTokenCommand(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }

    /// <summary>
    /// Command برای تغییر رمز عبور
    /// </summary>
    public class ChangePasswordCommand : IRequest<bool>
    {
        public string UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

        public ChangePasswordCommand(string userId, string currentPassword, string newPassword, string confirmPassword)
        {
            UserId = userId;
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
            ConfirmPassword = confirmPassword;
        }
    }
}
