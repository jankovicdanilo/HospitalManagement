using HospitalManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            this.appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await appointmentService.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await appointmentService.GetByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(new {result.Message, result.ErrorCode});
            }

            return Ok(result);
        }
    }
}
