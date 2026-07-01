using FluentValidation;
using HospitalManagement.Shared.Models.DTOs.Patient;
using HospitalManagement.Shared.Models.DTOs.Patient;

namespace HospitalManagement.CommandService.Validators.Patient
{
    public class PatientCreateRequestValidator : AbstractValidator<PatientCreateRequestDto>
    {
        public PatientCreateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Invalid email format");
        }
    }
}