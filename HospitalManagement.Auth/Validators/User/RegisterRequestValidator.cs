using FluentValidation;
using HospitalManagement.Auth.Models.DTOs;
using HospitalManagement.Auth.Models.Enums;

namespace HospitalManagement.Auth.Validators.User
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters")
                .MaximumLength(50).WithMessage("Username must not exceed 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

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
                    .Null().WithMessage("DoctorId is only allowed for the doctor role");
            });
        }
    }
}
