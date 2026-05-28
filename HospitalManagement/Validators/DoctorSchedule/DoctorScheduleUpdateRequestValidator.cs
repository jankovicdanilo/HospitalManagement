using FluentValidation;
using HospitalManagement.Models.DTOs.DoctorSchedule;

namespace HospitalManagement.Validators.DoctorSchedule
{
    public class DoctorScheduleUpdateRequestValidator : AbstractValidator<DoctorScheduleUpdateRequestDto>
    {
        public DoctorScheduleUpdateRequestValidator()
        {
            RuleFor(x => x.DayOfWeek).IsInEnum().WithMessage("Invalid day of week");
            RuleFor(x => x.StartHour).InclusiveBetween(8, 19).WithMessage("Start hour must be between 8 and 19");
            RuleFor(x => x.EndHour).InclusiveBetween(9, 20).WithMessage("End hour must be between 9 and 20")
                .GreaterThan(x => x.StartHour).WithMessage("End hour must be greater than start hour");
        }
    }
}
