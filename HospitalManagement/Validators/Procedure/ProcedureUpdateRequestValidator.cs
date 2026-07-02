using FluentValidation;
using HospitalManagement.Shared.Models.DTOs.Procedure;

namespace HospitalManagement.Validators.Procedure
{
    public class ProcedureUpdateRequestValidator : AbstractValidator<ProcedureUpdateRequestDto>
    {
        public ProcedureUpdateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0");
        }
    }
}
