using FluentValidation;
using HospitalManagement.Models.DTOs.Patient;
using HospitalManagement.Shared.Models.DTOs.Patient;

namespace HospitalManagement.Validators.Patient
{
    public class PatientUpdateRequestValidator : AbstractValidator<PatientUpdateRequestDto>
    {
        public PatientUpdateRequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id must be greater than 0");

            RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("First name cant have more than 100 characters");

            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100).WithMessage("Last name cant have more than 100 characters");

            RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email format");
        }
    }
}
