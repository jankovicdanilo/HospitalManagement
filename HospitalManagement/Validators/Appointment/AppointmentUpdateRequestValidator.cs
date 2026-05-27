using FluentValidation;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Models.Enums;

namespace HospitalManagement.Validators.Appointment
{
    public class AppointmentUpdateRequestValidator : AbstractValidator<AppointmentUpdateRequestDto>
    {
        public AppointmentUpdateRequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id must be greater than 0");

            RuleFor(x => x.DoctorId).GreaterThan(0).WithMessage("Doctor id must be greater than 0");

            RuleFor(x => x.PatientId).GreaterThan(0).WithMessage("Patient id must be greater than 0");

            RuleFor(x => x.DateTime)
                .GreaterThan(DateTime.UtcNow).WithMessage("Appointment date must be in the future");

            RuleFor(x => x.Duration)
                .GreaterThan(TimeSpan.Zero).WithMessage("Duration must be greater than 0")
                .LessThanOrEqualTo(TimeSpan.FromHours(8)).WithMessage("Duration cannot exceed 8 hours");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters");
        }
    }
}
