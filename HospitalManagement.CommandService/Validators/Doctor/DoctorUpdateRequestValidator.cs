using FluentValidation;
using HospitalManagement.Shared.Models.DTOs.Doctor;
using HospitalManagement.Shared.Models.DTOs.Doctor;

namespace HospitalManagement.CommandService.Validators.Doctor
{
    public class DoctorUpdateRequestValidator : AbstractValidator<DoctorUpdateRequestDto>
    {
        public DoctorUpdateRequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid doctor ID");
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Specialization).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Invalid email format");
            RuleFor(x => x.Phone).Matches(@"^\+?[0-9\s\-\(\)]{7,20}$")
                .When(x => !string.IsNullOrEmpty(x.Phone)).WithMessage("Invalid phone number format");
        }
    }
}