using FluentValidation;
using EventsManagement.Shared.DTOs;

namespace EventsManagement.Application.Validators
{
    /// <summary>
    /// Validator برای رویداد
    /// </summary>
    public class CreateEventValidator : AbstractValidator<CreateEventDto>
    {
        public CreateEventValidator()
        {
            // عنوان الزامی است
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("عنوان رویداد الزامی است")
                .MaximumLength(500)
                .WithMessage("عنوان رویداد نمی‌تواند بیشتر از 500 کاراکتر باشد");

            // منشا الزامی است
            RuleFor(x => x.EventSourceId)
                .GreaterThan(0)
                .WithMessage("منشا رویداد الزامی است");

            // موضوع الزامی است
            RuleFor(x => x.EventSubjectId)
                .GreaterThan(0)
                .WithMessage("موضوع رویداد الزامی است");

            // فوریت الزامی است
            RuleFor(x => x.UrgencyId)
                .GreaterThan(0)
                .WithMessage("فوریت رویداد الزامی است");

            // گستره وقوع الزامی است
            RuleFor(x => x.ScopeId)
                .GreaterThan(0)
                .WithMessage("گستره وقوع الزامی است");

            // گستره اثر الزامی است
            RuleFor(x => x.ImpactScopeId)
                .GreaterThan(0)
                .WithMessage("گستره اثر الزامی است");

            // محدوده اثر الزامی است
            RuleFor(x => x.ImpactRangeId)
                .GreaterThan(0)
                .WithMessage("محدوده اثر الزامی است");

            // تاریخ شروع الزامی است
            RuleFor(x => x.EventStartDate)
                .NotEmpty()
                .WithMessage("تاریخ شروع الزامی است");

            // تاریخ پایان نباید قبل از تاریخ شروع باشد
            RuleFor(x => x.EventEndDate)
                .GreaterThanOrEqualTo(x => x.EventStartDate)
                .When(x => x.EventEndDate.HasValue)
                .WithMessage("تاریخ پایان نمی‌تواند قبل از تاریخ شروع باشد");

            // واحد اقدام‌کننده الزامی است
            RuleFor(x => x.ActionUnitId)
                .GreaterThan(0)
                .WithMessage("واحد اقدام‌کننده الزامی است");
        }
    }
}
