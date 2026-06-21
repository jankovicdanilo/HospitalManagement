using FluentValidation;
using HospitalManagement.CommandService.Models.Doctor;
using HospitalManagement.CommandService.Models.DTOs.Doctor;
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

            return Ok(result);
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
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await doctorService.Delete(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}