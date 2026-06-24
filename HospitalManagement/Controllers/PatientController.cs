using FluentValidation;
using HospitalManagement.Models.DTOs.Patient;
using HospitalManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Shared.Controllers;

namespace HospitalManagement.Controllers
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await patientService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var result = await patientService.GetByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(new { result.Message, result.ErrorCode });
            }
                
            return Ok(result);
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

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
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