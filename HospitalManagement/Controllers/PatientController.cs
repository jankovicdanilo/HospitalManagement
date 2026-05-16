using HospitalManagement.Models.DTOs.Patient;
using HospitalManagement.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HospitalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService patientService;
        public PatientController(IPatientService patientService)
        {
            this.patientService = patientService;
        }

        [HttpGet]
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
                return NotFound(new { result.Message, result.ErrorCode });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreatePatientRequestDto request)
        {
            var result = await patientService.CreateAsync(request);
            if (!result.Success)
                return NotFound(new { result.Message, result.ErrorCode });
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(PatientUpdateRequestDto request)
        {
            var result = await patientService.UpdateAsync(request);
            if (!result.Success)
                return NotFound(new { result.Message, result.ErrorCode });
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await patientService.Delete(id);
            if (!result.Success)
                return NotFound(new { result.Message, result.ErrorCode });
            return Ok(result);
        }
    }
}