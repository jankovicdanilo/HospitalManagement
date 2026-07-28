using FluentValidation;
using HospitalManagement.Shared.Models.DTOs.DoctorSchedule;
using HospitalManagement.CommandService.Services.Interfaces;
using HospitalManagement.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.CommandService.Controllers
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
                return HandleFailure(result);
            }

            return Ok(result.Data);
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
                return HandleFailure(result);
            }

            return Ok(result.Data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await doctorScheduleService.Delete(id);

            if (!result.Success)
            {
                return HandleFailure(result);
            }

            return Ok(new {result.Message} );
        }
    }
}