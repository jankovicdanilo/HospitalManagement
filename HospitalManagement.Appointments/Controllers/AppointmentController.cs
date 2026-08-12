using FluentValidation;
using HospitalManagement.Appointments.Models.DTOs.Appointment;
using HospitalManagement.Appointments.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Shared.Controllers;

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

        [HttpDelete]
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
        public async Task<IActionResult> GetPatientHistoryAsync([FromRoute] int patientId)
        {
            var result = await appointmentService.GetPatientHistoryAsync(patientId);

            if (!result.Success)
            {
                return HandleFailure(result);
            }

            return Ok(result.Data);
        }
    }
}