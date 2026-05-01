using FluentValidation;
using HospitalManagement.Models.DTOs.Doctor;

namespace HospitalManagement.Validators.Doctor
{
    public class DoctorUpdateRequestValidator : AbstractValidator<DoctorUpdateRequestDto>
    {
        public DoctorUpdateRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid doctor ID");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");

            RuleFor(x => x.Specialization)
                .NotEmpty().WithMessage("Specialization is required")
                .MaximumLength(100).WithMessage("Specialization must not exceed 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Phone)
                .Matches(@"^\+?[0-9\s\-\(\)]{7,20}$")
                .When(x => !string.IsNullOrEmpty(x.Phone))
                .WithMessage("Invalid phone number format");
        }
    }
}
