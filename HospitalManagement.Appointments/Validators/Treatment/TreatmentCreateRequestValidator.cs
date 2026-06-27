using FluentValidation;
using HospitalManagement.Appointments.Models.DTOs.Treatment;

namespace HospitalManagement.Appointments.Validators.Treatment
{
    public class TreatmentCreateRequestValidator : AbstractValidator<TreatmentCreateRequestDto>
    {
        public TreatmentCreateRequestValidator()
        {
            RuleFor(x => x.AppointmentId)
                .GreaterThan(0).WithMessage("AppointmentId must be valid");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.Medication)
                .MaximumLength(500).WithMessage("Medication cannot exceed 500 characters")
                .When(x => x.Medication != null);
        }
    }
}