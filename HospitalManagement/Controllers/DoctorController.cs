using FluentValidation;
using HospitalManagement.Models.DTOs.Doctor;
using HospitalManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
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
        public async Task<IActionResult> Create(
            [FromBody] DoctorCreateRequestDto request,
            [FromServices] IValidator<DoctorCreateRequestDto> validator)
        {
            var validation = await validator.ValidateAsync(request);

            if(!validation.IsValid)
            {
                return ValidationFailed(validation);
            }

            var result = await doctorService.Create(request);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await doctorService.GetAll();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await doctorService.GetById(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(
            [FromBody] DoctorUpdateRequestDto request,
            [FromServices] IValidator<DoctorUpdateRequestDto> validator)
        {
            var validation = await validator.ValidateAsync(request);

            if(!validation.IsValid)
            {
                return ValidationFailed(validation);
            }

            var result = await doctorService.Update(request);

            if(!result.Success)
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
