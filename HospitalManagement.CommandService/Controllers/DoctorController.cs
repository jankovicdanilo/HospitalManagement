using FluentValidation;
using HospitalManagement.Shared.Models.DTOs.Doctor;
using HospitalManagement.Shared.Models.DTOs.Doctor;
using HospitalManagement.CommandService.Services.Interfaces;
using HospitalManagement.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.CommandService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : BaseController
    {
        private readonly IDoctorService doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            this.doctorService = doctorService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(
            [FromBody] DoctorCreateRequestDto request,
            [FromServices] IValidator<DoctorCreateRequestDto> validator)
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            { 
                return ValidationFailed(validation); 
            }

            var result = await doctorService.CreateAsync(request);
            if (!result.Success)
            {
                return HandleFailure(result);
            }

            return Ok(result.Data);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(
            [FromBody] DoctorUpdateRequestDto request,
            [FromServices] IValidator<DoctorUpdateRequestDto> validator)
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return ValidationFailed(validation);
            }

            var result = await doctorService.UpdateAsync(request);

            if (!result.Success)
            {
                return HandleFailure(result);
            }

            return Ok(result.Data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await doctorService.Delete(id);

            if (!result.Success)
            {
                return HandleFailure(result);
            }

            return Ok(new { result.Message });
        }
    }
}