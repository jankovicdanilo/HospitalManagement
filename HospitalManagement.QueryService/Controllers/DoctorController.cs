using FluentValidation;
using HospitalManagement.QueryService.Services.Interfaces;
using HospitalManagement.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Shared.Models.DTOs.Common;

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
        public async Task<IActionResult> GetAllAsync([FromQuery] PageQueryDto query, [FromServices] IValidator<PageQueryDto> validator)
        {
            var validation = await validator.ValidateAsync(query);
            if (!validation.IsValid)
            {
                return ValidationFailed(validation);
            }

            var result = await doctorService.GetAllAsync(query.PageNumber, query.PageSize);

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
    }
}