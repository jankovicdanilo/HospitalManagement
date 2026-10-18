using FluentValidation;
using HospitalManagement.Auth.Models.DTOs;

namespace HospitalManagement.Auth.Validators.ServiceToken
{
    public class ServiceTokenCreateRequestValidator : AbstractValidator<ServiceTokenCreateRequestDto>
    {
        public ServiceTokenCreateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(50)
                .Matches("^[a-zA-Z0-9 _-]+$")
                .WithMessage("Name may only contain letters, digits, spaces, - and _");

            RuleFor(x => x.ExpiresInDays)
                .InclusiveBetween(1, 90)
                .WithMessage("Expiry must be between 1 and 90 days");
        }
    }
}
