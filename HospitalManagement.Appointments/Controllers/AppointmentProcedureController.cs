using HospitalManagement.Appointments.Models.DTOs.AppointmentProcedure;
using HospitalManagement.Appointments.Services.Interfaces;
using HospitalManagement.Shared.Controllers;
using HospitalManagement.Shared.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Appointments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = PolicyNames.AppointmentAccess)]
    public class AppointmentProcedureController : BaseController
    {
        private readonly IAppointmentProcedureService appointmentProcedureService;
        private readonly IAppointmentAccess appointmentAccess;

        public AppointmentProcedureController(IAppointmentProcedureService appointmentProcedureService, IAppointmentAccess appointmentAccess)
        {
            this.appointmentProcedureService = appointmentProcedureService;
            this.appointmentAccess = appointmentAccess;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] AppointmentProcedureCreateRequestDto request)
        {
            var access = await appointmentAccess.CheckAppointmentAsync(User, request.AppointmentId);
            if (!access.Success)
            {
                return HandleFailure(access);
            }

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
            var access = await appointmentAccess.CheckAppointmentAsync(User, appointmentId);
            if (!access.Success)
            {
                return HandleFailure(access);
            }

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
            var access = await appointmentAccess.CheckAppointmentAsync(User, appointmentId);
            if (!access.Success)
            {
                return HandleFailure(access);
            }

            var result = await appointmentProcedureService.DeleteAsync(appointmentId, procedureId);

            if (!result.Success)
            {
                return HandleFailure(result);
            }
                
            return Ok(result.Data);
        }
    }
}