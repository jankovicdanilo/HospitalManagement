using FluentValidation;
using HospitalManagement.Shared.Models.DTOs.Procedure;

namespace HospitalManagement.Validators.Procedure
{
    public class ProcedureCreateRequestValidator :AbstractValidator<ProcedureCreateRequestDto>
    {
        public ProcedureCreateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Price must be greater than 0");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0");
        }
    }
}
