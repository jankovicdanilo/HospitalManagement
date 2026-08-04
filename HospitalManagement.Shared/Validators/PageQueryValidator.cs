using FluentValidation;
using HospitalManagement.Shared.Models.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Shared.Validators
{
    public class PageQueryValidator : AbstractValidator<PageQueryDto>
    {
        public PageQueryValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("Page number must be greater than 0.");
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
        }
    }
}
