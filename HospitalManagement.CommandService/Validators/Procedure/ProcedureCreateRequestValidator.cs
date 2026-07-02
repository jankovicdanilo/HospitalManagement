using FluentValidation;
using HospitalManagement.Shared.Models.DTOs.Procedure;

namespace HospitalManagement.CommandService.Validators.Procedure
{
    public class ProcedureCreateRequestValidator : AbstractValidator<ProcedureCreateRequestDto>
    {
        public ProcedureCreateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
        }
    }
}