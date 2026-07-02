using HospitalManagement.QueryService.Services.Interfaces;
using HospitalManagement.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.QueryService.Controllers
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

        [HttpGet("doctor/{doctorId:int}/day/{dayOfWeek}")]
        public async Task<IActionResult> GetByDoctorIdAndDayAsync([FromRoute] int doctorId, [FromRoute] DayOfWeek dayOfWeek)
        {
            var result = await doctorScheduleService.GetByDoctorIdAndDayAsync(doctorId, dayOfWeek);

            if (!result.Success)
            {
                return NotFound(new { result.Message, result.ErrorCode });
            }

            return Ok(result);
        }
    }
}