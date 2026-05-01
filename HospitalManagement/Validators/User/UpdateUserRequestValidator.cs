using FluentValidation;
using HospitalManagement.Models.DTOs.Auth;
using System.Collections.Generic;

namespace HospitalManagement.Validators.User
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
        }
    }
}
