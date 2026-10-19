using FluentValidation;
using HospitalManagement.Shared.Models.DTOs.DoctorSchedule;

namespace HospitalManagement.CommandService.Validators.DoctorSchedule
{
    public class DoctorScheduleCreateRequestValidator : AbstractValidator<DoctorScheduleCreateRequestDto>
    {
        public DoctorScheduleCreateRequestValidator()
        {
            RuleFor(x => x.DoctorId).GreaterThan(0).WithMessage("DoctorId must be greater than 0");
            RuleFor(x => x.DayOfWeek).IsInEnum().WithMessage("Invalid day of week")
                .Must(x => x != DayOfWeek.Saturday && x != DayOfWeek.Sunday)
                .WithMessage("Doctor schedule cannot be set for weekends");
            RuleFor(x => x.StartHour).InclusiveBetween(0, 23).WithMessage("Start hour must be between 0 and 23 (UTC)");
            RuleFor(x => x.EndHour).InclusiveBetween(1, 24).WithMessage("End hour must be between 1 and 24 (UTC)")
                .GreaterThan(x => x.StartHour).WithMessage("End hour must be greater than start hour");
        }
    }
}