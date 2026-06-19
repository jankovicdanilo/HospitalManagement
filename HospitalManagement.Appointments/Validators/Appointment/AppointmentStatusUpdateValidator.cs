using FluentValidation;
using HospitalManagement.Appointments.Models.DTOs.Appointment;
using HospitalManagement.Appointments.Models.Enums;

namespace HospitalManagement.Appointments.Validators.Appointment
{
    public class AppointmentStatusUpdateValidator : AbstractValidator<AppointmentStatusUpdateDto>
    {
        public AppointmentStatusUpdateValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id must be greater than 0");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status value")
                .Must(x => x == AppointmentStatus.Completed || x == AppointmentStatus.Cancelled)
                .WithMessage("Status can only be set to Completed or Cancelled manually");
        }
    }
}