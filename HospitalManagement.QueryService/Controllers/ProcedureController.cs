using HospitalManagement.QueryService.Services.Interfaces;
using HospitalManagement.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.QueryService.Controllers
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

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await procedureService.GetAllAsync();

            if (!result.Success)
            {
                return HandleFailure(result);
            }

            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var result = await procedureService.GetByIdAsync(id);

            if (!result.Success)
            {
                return HandleFailure(result);
            }

            return Ok(result.Data);
        }
    }
}