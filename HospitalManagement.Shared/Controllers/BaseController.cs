using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.Shared.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult HandleFailure(Result result)
        {
            var body = new { result.Message, result.ErrorCode };
            return MapErrorType(result.ErrorType, body);
        }

        protected IActionResult HandleFailure<T>(Result<T> result)
        {
            var body = new {result.Message, result.ErrorCode};
            return MapErrorType(result.ErrorType, body);
        }

        protected IActionResult ValidationFailed(FluentValidation.Results.ValidationResult validation)
        {
            var logger = (ILogger)HttpContext.RequestServices.GetRequiredService(typeof(ILogger<>).MakeGenericType(GetType()));

            var errors = string.Join(", ", validation.Errors.Select(x => x.ErrorMessage));
            logger.LogWarning("Validation failed: {Errors}", errors);

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

        private IActionResult MapErrorType(ErrorType? errorType, object body) => errorType switch
        {
            ErrorType.NotFound => NotFound(body),
            ErrorType.Validation => BadRequest(body),
            ErrorType.Conflict => Conflict(body),
            ErrorType.Unauthorized => Unauthorized(body),
            ErrorType.UpstreamFailure => StatusCode(502, body),
            _ => StatusCode(500, body)
        };
    }
}
