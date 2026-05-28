using FluentValidation;
using HospitalManagement.Models.DTOs.DoctorSchedule;

namespace HospitalManagement.Validators.DoctorSchedule
{
    public class DoctorScheduleCreateRequestValidator : AbstractValidator<DoctorScheduleCreateRequestDto>
    {
        public DoctorScheduleCreateRequestValidator()
        {
            RuleFor(x => x.DoctorId).GreaterThan(0).WithMessage("DoctorId must be greater than 0");
            RuleFor(x => x.DayOfWeek).IsInEnum().WithMessage("Invalid day of week");
            RuleFor(x => x.StartHour).InclusiveBetween(0, 23).WithMessage("Start hour must be between 0 and 23");
            RuleFor(x => x.EndHour).InclusiveBetween(0, 23).WithMessage("End hour must be between 0 and 23")
                .GreaterThan(x => x.StartHour).WithMessage("End hour must be greater than start hour");
        }
    }
}
