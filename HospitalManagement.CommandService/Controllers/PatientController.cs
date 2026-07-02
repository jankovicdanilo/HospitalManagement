using FluentValidation;
using HospitalManagement.Shared.Models.DTOs.Patient;
using HospitalManagement.Shared.Models.DTOs.Patient;
using HospitalManagement.CommandService.Services.Interfaces;
using HospitalManagement.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.CommandService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : BaseController
    {
        private readonly IPatientService patientService;

        public PatientController(IPatientService patientService)
        {
            this.patientService = patientService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] PatientCreateRequestDto request,
            [FromServices] IValidator<PatientCreateRequestDto> validator)
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return ValidationFailed(validation);
            }

            var result = await patientService.CreateAsync(request);

            if (!result.Success)
            {
                return NotFound(new { result.Message, result.ErrorCode });
            }

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] PatientUpdateRequestDto request,
            [FromServices] IValidator<PatientUpdateRequestDto> validator)
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return ValidationFailed(validation);
            }

            var result = await patientService.UpdateAsync(request);

            if (!result.Success)
            {
                return NotFound(new { result.Message, result.ErrorCode });
            }

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await patientService.Delete(id);

            if (!result.Success)
            {
                return NotFound(new { result.Message, result.ErrorCode });
            }

            return Ok(result);
        }
    }
}