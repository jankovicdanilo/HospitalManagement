using FluentValidation;
using HospitalManagement.Models.DTOs.DoctorSchedule;
using HospitalManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorScheduleController : BaseController
    {
        private readonly IDoctorScheduleService doctorScheduleService;

        public DoctorScheduleController(IDoctorScheduleService doctorScheduleService)
        {
            this.doctorScheduleService = doctorScheduleService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] DoctorScheduleCreateRequestDto request,
            [FromServices] IValidator<DoctorScheduleCreateRequestDto> validator)
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return ValidationFailed(validation);
            }

            var result = await doctorScheduleService.CreateAsync(request);

            if (!result.Success)
            {
                return BadRequest(new { result.Message, result.ErrorCode });
            }

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var result = await doctorScheduleService.GetByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(new { result.Message, result.ErrorCode });
            }

            return Ok(result);
        }

        [HttpGet("doctor/{doctorId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllByDoctorIdAsync([FromRoute] int doctorId)
        {
            var result = await doctorScheduleService.GetAllByDoctorIdAsync(doctorId);

            if (!result.Success)
            {
                return NotFound(new { result.Message, result.ErrorCode });
            }

            return Ok(result);
        }
    }
}
