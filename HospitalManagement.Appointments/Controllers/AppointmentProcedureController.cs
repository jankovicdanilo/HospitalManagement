using HospitalManagement.Appointments.Models.DTOs.AppointmentProcedure;
using HospitalManagement.Appointments.Services.Interfaces;
using HospitalManagement.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Appointments.Controllers
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

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] AppointmentProcedureCreateRequestDto request)
        {
            var result = await appointmentProcedureService.CreateAsync(request);

            if (!result.Success)
            {
                return HandleFailure(result);
            }
                
            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetByAppointmentAndProcedureIdAsync(int appointmentId, int procedureId)
        {
            var result = await appointmentProcedureService.GetByAppointmentAndProcedureIdAsync(appointmentId, procedureId);

            if (!result.Success)
            {
                return HandleFailure(result);
            }
                
            return Ok(result.Data);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int appointmentId, int procedureId)
        {
            var result = await appointmentProcedureService.DeleteAsync(appointmentId, procedureId);

            if (!result.Success)
            {
                return HandleFailure(result);
            }
                
            return Ok(result.Data);
        }
    }
}