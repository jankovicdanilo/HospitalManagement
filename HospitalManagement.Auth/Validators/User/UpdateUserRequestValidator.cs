using FluentValidation;
using HospitalManagement.Auth.Models.DTOs;
using HospitalManagement.Auth.Models.Enums;

namespace HospitalManagement.Auth.Validators.User
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequestDto>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid user ID");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters")
                .MaximumLength(50).WithMessage("Username must not exceed 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Invalid role");

            When(x => x.Role == UserRole.Doctor, () =>
            {
                RuleFor(x => x.DoctorId)
                    .NotNull().WithMessage("DoctorId is required for the Doctor role")
                    .GreaterThan(0).WithMessage("DoctorId must be greater than 0");
            });

            When(x => x.Role != UserRole.Doctor, () =>
            {
                RuleFor(x => x.DoctorId)
                    .Null()
                    .WithMessage("DoctorId is only allowed for the Doctor role");
            });
        }
    }
}
