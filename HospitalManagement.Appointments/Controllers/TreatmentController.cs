using FluentValidation;
using HospitalManagement.Appointments.Models.DTOs.Treatment;
using HospitalManagement.Appointments.Services.Implementations;
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
    public class TreatmentController : BaseController
    {
        private readonly ITreatmentService treatmentService;
        private readonly IAppointmentAccess appointmentAccess;

        public TreatmentController(ITreatmentService treatmentService, IAppointmentAccess appointmentAccess)
        {
            this.treatmentService = treatmentService;
            this.appointmentAccess = appointmentAccess;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] TreatmentCreateRequestDto request,
            [FromServices] IValidator<TreatmentCreateRequestDto> validator)
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return ValidationFailed(validation);
            }

            var access = await appointmentAccess.CheckAppointmentAsync(User, request.AppointmentId);
            if (!access.Success)
            {
                return HandleFailure(access);
            }

            var result = await treatmentService.CreateAsync(request);

            if (!result.Success)
            {
                return HandleFailure(result);
            }
                
            return Ok(result.Data);
        }
    }
}