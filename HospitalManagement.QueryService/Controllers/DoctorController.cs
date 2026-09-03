using FluentValidation;
using HospitalManagement.QueryService.Services.Interfaces;
using HospitalManagement.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Shared.Models.DTOs.Common;
using HospitalManagement.Shared.Models.DTOs.Doctor;

namespace HospitalManagement.QueryService.Controllers
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

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] DoctorFilterDto filter, [FromServices] IValidator<DoctorFilterDto> validator)
        {
            var validation = await validator.ValidateAsync(filter);
            if (!validation.IsValid)
            {
                return ValidationFailed(validation);
            }

            var result = await doctorService.GetAllAsync(filter);

            if (!result.Success)
            {
                return HandleFailure(result);
            }

            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var result = await doctorService.GetByIdAsync(id);

            if (!result.Success)
            {
                return HandleFailure(result);
            }

            return Ok(result.Data);
        }

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularAsync([FromQuery] int count = 5)
        {
            var result = await doctorService.GetPopularDoctorsAsync(count);

            return result.Success ? Ok(result.Data) : HandleFailure(result);
        }
    }
}