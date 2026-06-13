using FluentValidation;
using HospitalManagement.Models.DTOs.Treatment;
using HospitalManagement.Services.Interfaces;
using HospitalManagement.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreatmentController : BaseController
    {
        private readonly ITreatmentService treatmentService;

        public TreatmentController(ITreatmentService treatmentService)
        {
            this.treatmentService = treatmentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] TreatmentCreateRequestDto request,
            [FromServices] IValidator<TreatmentCreateRequestDto> validator)
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return ValidationFailed(validation);
            }

            var result = await treatmentService.CreateAsync(request);

            if (!result.Success)
            {
                return NotFound(new { result.Message, result.ErrorCode });
            }

            return Ok(result);
        }
    }
}
