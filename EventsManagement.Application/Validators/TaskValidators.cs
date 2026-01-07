using FluentValidation;
using EventsManagement.Shared.DTOs;

namespace EventsManagement.Application.Validators
{
    /// <summary>
    /// Validator برای وظیفه
    /// </summary>
    public class CreateTaskValidator : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskValidator()
        {
            // عنوان الزامی است
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("عنوان وظیفه الزامی است")
                .MaximumLength(500)
                .WithMessage("عنوان وظیفه نمی‌تواند بیشتر از 500 کاراکتر باشد");

            // رویداد الزامی است
            RuleFor(x => x.EventId)
                .GreaterThan(0)
                .WithMessage("رویداد الزامی است");

            // واحد الزامی است
            RuleFor(x => x.AssignedToUnitId)
                .GreaterThan(0)
                .WithMessage("واحد اختصاص‌داده‌شده الزامی است");

            // نوع اقدام الزامی است
            RuleFor(x => x.ActionTypeId)
                .GreaterThan(0)
                .WithMessage("نوع اقدام الزامی است");
        }
    }
}
