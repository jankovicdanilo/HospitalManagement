using FluentValidation;
using HospitalManagement.Shared.Models.DTOs.DoctorSchedule;
using HospitalManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using HospitalManagement.Shared.Controllers;
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
        public async Task<IActionResult> GetAllByDoctorIdAsync([FromRoute] int doctorId)
        {
            var result = await doctorScheduleService.GetAllByDoctorIdAsync(doctorId);

            if (!result.Success)
            {
                return NotFound(new { result.Message, result.ErrorCode });
            }

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await doctorScheduleService.Delete(id);

            if (!result.Success)
            {
                return NotFound(new {result.Message, result.ErrorCode});
            }

            return Ok(result);
        }

        [HttpGet("doctor/{doctorId:int}/day/{dayOfWeek}")]
        public async Task<IActionResult> GetByDoctorIdAndDayAsync(int doctorId, DayOfWeek dayOfWeek)
        {
            var result = await doctorScheduleService.GetByDoctorIdAndDayAsync(doctorId, dayOfWeek);

            if (!result.Success)
            {
                return NotFound(new { result.Message, result.ErrorCode });
            }

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] DoctorScheduleUpdateRequestDto request,
            [FromServices] IValidator<DoctorScheduleUpdateRequestDto> validator)
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return ValidationFailed(validation);
            }

            var result = await doctorScheduleService.UpdateAsync(request);

            if (!result.Success)
            {
                return BadRequest(new { result.Message, result.ErrorCode });
            }

            return Ok(result);
        }
    }
}
