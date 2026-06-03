using HospitalManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentProcedureController : BaseController
    {
        private readonly IAppointmentProcedureService appointmentProcedureService;

        public AppointmentProcedureController(IAppointmentProcedureService appointmentProcedureService)
        {
            this.appointmentProcedureService = appointmentProcedureService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAsync(int appointmentId, int procedureId)
        {
            var result = await appointmentProcedureService.GetAsync(appointmentId, procedureId);

            if (!result.Success)
            {
                return NotFound(new { result.Message, result.ErrorCode });
            }

            return Ok(result);
        }
    }
}
