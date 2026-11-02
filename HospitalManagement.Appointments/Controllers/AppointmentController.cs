using FluentValidation;
using HospitalManagement.Appointments.Models.DTOs.Appointment;
using HospitalManagement.Appointments.Services.Interfaces;
using HospitalManagement.Shared.Controllers;
using HospitalManagement.Shared.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Appointments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : BaseController
    {
        private readonly IAppointmentService appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            this.appointmentService = appointmentService;
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = PolicyNames.AppointmentAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await appointmentService.Delete(id);

            if (!result.Success)
            {
                return HandleFailure(result);
            }
                
            return Ok(new { result.Message });
        }

        [HttpPut]
        [Authorize(Policy = PolicyNames.AppointmentAccess)]
        public async Task<IActionResult> UpdateAsync([FromBody] AppointmentUpdateRequestDto request,
            [FromServices] IValidator<AppointmentUpdateRequestDto> validator)
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return ValidationFailed(validation);
            }
                

            var result = await appointmentService.UpdateAsync(request);

            if (!result.Success)
            {
                return HandleFailure(result);
            }
                
            return Ok(result.Data);
        }

        [HttpGet]
        [Authorize(Policy = PolicyNames.AppointmentAccess)]
        public async Task<IActionResult> GetAllAsync([FromQuery] AppointmentFilterDto filter,
            [FromServices] IValidator<AppointmentFilterDto> validator)
        {
            var validation = await validator.ValidateAsync(filter);
            if (!validation.IsValid)
            {
                return ValidationFailed(validation);
            }

            var result = await appointmentService.GetAllAsync(filter);

            if (!result.Success)
            {
                return HandleFailure(result);
            }

            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = PolicyNames.AppointmentAccess)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await appointmentService.GetByIdAsync(id);

            if (!result.Success)
            {
                return HandleFailure(result);
            }
                

            return Ok(result.Data);
        }

        [HttpPost]
        [Authorize(Policy = PolicyNames.AppointmentAccess)]
        public async Task<IActionResult> CreateAsync([FromBody] AppointmentCreateRequestDto request,
            [FromServices] IValidator<AppointmentCreateRequestDto> validator)
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return ValidationFailed(validation);

            }

            var result = await appointmentService.CreateAsync(request);

            if (!result.Success)
            {
                return HandleFailure(result);
            }
                
            return Ok(result.Data);
        }

        [HttpGet("free-slots")]
        [Authorize(Policy = PolicyNames.AppointmentAccess)]
        public async Task<IActionResult> FreeSlots([FromQuery] FreeSlotsRequestDto request,
            [FromServices] IValidator<FreeSlotsRequestDto> validator)
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return ValidationFailed(validation);
            }
                
            var result = await appointmentService.GetFreeSlotsAsync(request.DoctorId, request.Date);

            if (!result.Success)
            {
                return HandleFailure(result);
            }
                
            return Ok(result.Data);
        }

        [HttpPatch("status")]
        [Authorize(Policy = PolicyNames.AppointmentAccess)]
        public async Task<IActionResult> UpdateStatusAsync([FromBody] AppointmentStatusUpdateDto request,
            [FromServices] IValidator<AppointmentStatusUpdateDto> validator)
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return ValidationFailed(validation);
            }
                
            var result = await appointmentService.UpdateStatusAsync(request);

            if (!result.Success)
            {
                return HandleFailure(result);
            }
                
            return Ok(new { result.Message });
        }

        [HttpGet("patient/{patientId:int}/history")]
        [Authorize(Policy = PolicyNames.AppointmentAccess)]
        public async Task<IActionResult> GetPatientHistoryAsync([FromRoute] int patientId)
        {
            var result = await appointmentService.GetPatientHistoryAsync(patientId);

            if (!result.Success)
            {
                return HandleFailure(result);
            }

            return Ok(result.Data);
        }

        [HttpGet("doctors/popular-ids")]
        public async Task<IActionResult> GetPopularDoctorIdsAsync([FromQuery] int count = 5)
        {
            var result = await appointmentService.GetPopularDoctorIdsAsync(count);

            return result.Success ? Ok(result.Data) : HandleFailure(result);
        }

        [HttpGet("patients/popular-ids")]
        public async Task<IActionResult> GetPopularPatientIdsAsync([FromQuery] int count = 5)
        {
            var result = await appointmentService.GetPopularPatientIdsAsync(count);

            return result.Success ? Ok(result.Data) : HandleFailure(result);
        }

        [HttpGet("patient/{patientId:int}/summary")]
        [Authorize(Policy = PolicyNames.AppointmentAccess)]
        public async Task<IActionResult> GetPatientSummaryAsync([FromRoute] int patientId, [FromQuery] string language = "en")
        {
            var result = await appointmentService.GetPatientSummaryAsync(patientId, language);

            return result.Success ? Ok(result.Data) : HandleFailure(result);
        }

        [HttpGet("stats-data")]
        [Authorize(Policy = PolicyNames.ReportsAccess)]
        public async Task<IActionResult> GetStatsDataAsync([FromQuery] DateOnly from, [FromQuery] DateOnly to)
        {
            var result = await appointmentService.GetStatsDataAsync(from, to);
            return result.Success ? Ok(result.Data) : HandleFailure(result);
        }

        [HttpGet("{id:int}/invoice")]
        [Authorize(Policy = PolicyNames.ReportsAccess)]
        public async Task<IActionResult> GetForInvoiceAsync([FromRoute] int id)
        {
            var result = await appointmentService.GetByIdAsync(id);

            return result.Success ? Ok(result.Data) : HandleFailure(result);
        }
    }
}