using FluentValidation;
using HospitalManagement.Models.DTOs.Patient;

namespace HospitalManagement.Validators.Patient
{
    public class PatientCreateRequestValidator : AbstractValidator<PatientCreateRequestDto>
    {
        public PatientCreateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);

            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);

            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}
