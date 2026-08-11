using FluentValidation;
using HospitalManagement.Appointments.Models.DTOs.Appointment;

namespace HospitalManagement.Appointments.Validators.Appointment
{
    public class AppointmentFilterValidator : AbstractValidator<AppointmentFilterDto>
    {
        public AppointmentFilterValidator()
        {
            RuleFor(x => x)
                .Must(x => (x.StartDate.HasValue && x.EndDate.HasValue) || (!x.StartDate.HasValue && !x.EndDate.HasValue))
                .WithMessage("StartDate and EndDate must both be provided together, or both omitted");

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("EndDate must be on or after StartDate");

            RuleFor(x => x.PageNumber).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        }
    }
}
