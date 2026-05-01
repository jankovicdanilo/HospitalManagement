using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;

namespace HospitalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult ValidationFailed(ValidationResult validation)
        {
            return BadRequest(new
            {
                ErrorCode = "VALIDATION_FAILED",
                Message = "One or more validation errors occurred",
                Errors = validation.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray()
                    )
            });
        }
    }
}
