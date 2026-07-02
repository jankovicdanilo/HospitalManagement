using FluentValidation;
using HospitalManagement.Shared.Models.DTOs.Procedure;
using HospitalManagement.Shared.Controllers;
using HospitalManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HospitalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcedureController : BaseController
    {
        private readonly IProcedureService procedureService;

        public ProcedureController(IProcedureService procedureService)
        {
            this.procedureService = procedureService;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] ProcedureUpdateRequestDto request,
            [FromServices] IValidator<ProcedureUpdateRequestDto> validator)
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return ValidationFailed(validation);
            }

            var result = await procedureService.UpdateAsync(id, request);

            if (!result.Success)
            {
                return BadRequest(new { result.Message, result.ErrorCode });
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ProcedureCreateRequestDto request,
            [FromServices] IValidator<ProcedureCreateRequestDto> validator)
        {
            var validation = await validator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                return ValidationFailed(validation);
            }
            
            var result = await procedureService.CreateAsync(request);

            if (!result.Success)
            {
                return BadRequest(new {result.Message, result.ErrorCode});
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await procedureService.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var result = await procedureService.GetByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(new { result.Message, result.ErrorCode });
            }

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await procedureService.DeleteAsync(id);

            if (!result.Success)
            {
                return NotFound(new {result.Message, result.ErrorCode});
            }

            return Ok(result);
        }
    }
}
