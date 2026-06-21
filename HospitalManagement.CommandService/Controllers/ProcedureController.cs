using FluentValidation;
using HospitalManagement.CommandService.Models.Procedure;
using HospitalManagement.CommandService.Services.Interfaces;
using HospitalManagement.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.CommandService.Controllers
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
                return BadRequest(new { result.Message, result.ErrorCode });
            }

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id,
            [FromBody] ProcedureUpdateRequestDto request,
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

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var result = await procedureService.DeleteAsync(id);

            if (!result.Success)
            {
                return NotFound(new { result.Message, result.ErrorCode });
            }
            return Ok(result);
        }
    }
}