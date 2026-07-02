using FluentValidation;
using HospitalManagement.Shared.Models.DTOs.Doctor;

namespace HospitalManagement.CommandService.Validators.Doctor
{
    public class DoctorCreateRequestValidator : AbstractValidator<DoctorCreateRequestDto>
    {
        public DoctorCreateRequestValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Specialization).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Invalid email format");
            RuleFor(x => x.Phone).Matches(@"^\+?[0-9\s\-\(\)]{7,20}$")
                .When(x => !string.IsNullOrEmpty(x.Phone)).WithMessage("Invalid phone number format");
        }
    }
}