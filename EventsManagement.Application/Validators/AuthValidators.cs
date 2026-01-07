using FluentValidation;
using EventsManagement.Shared.DTOs;

namespace EventsManagement.Application.Validators
{
    /// <summary>
    /// Validator برای ورود
    /// </summary>
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            // نام کاربری نمی‌تواند خالی باشد
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("نام کاربری الزامی است");

            // رمز عبور نمی‌تواند خالی باشد
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("رمز عبور الزامی است");
        }
    }

    /// <summary>
    /// Validator برای تغییر رمز عبور
    /// </summary>
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage("رمز عبور فعلی الزامی است");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("رمز عبور جدید الزامی است")
                .MinimumLength(8)
                .WithMessage("رمز عبور جدید باید حداقل 8 کاراکتر باشد")
                .Matches(@"[A-Z]")
                .WithMessage("رمز عبور باید شامل حداقل یک حرف بزرگ انگلیسی باشد")
                .Matches(@"[a-z]")
                .WithMessage("رمز عبور باید شامل حداقل یک حرف کوچک انگلیسی باشد")
                .Matches(@"[0-9]")
                .WithMessage("رمز عبور باید شامل حداقل یک رقم باشد");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword)
                .WithMessage("رمز عبور جدید و تائید آن مطابقت ندارند");
        }
    }
}
