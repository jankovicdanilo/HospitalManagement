using FluentValidation;
using HospitalManagement.Appointments.Models.DTOs.Appointment;

namespace HospitalManagement.Appointments.Validators.Appointment
{
    public class FreeSlotsRequestValidator : AbstractValidator<FreeSlotsRequestDto>
    {
        public FreeSlotsRequestValidator()
        {
            RuleFor(x => x.DoctorId).GreaterThan(0).WithMessage("DoctorId is required");
            RuleFor(x => x.Date).NotEqual(DateOnly.MinValue).WithMessage("Date is required");
        }
    }
}