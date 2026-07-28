using HospitalManagement.QueryService.Services.Interfaces;
using HospitalManagement.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.QueryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : BaseController
    {
        private readonly IPatientService patientService;

        public PatientController(IPatientService patientService)
        {
            this.patientService = patientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await patientService.GetAllAsync();

            if (!result.Success)
            {
                return HandleFailure(result);
            }

            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var result = await patientService.GetByIdAsync(id);

            if (!result.Success)
            {
                return HandleFailure(result);
            }

            return Ok(result.Data);
        }

        [HttpGet("{patientId:int}/history")]
        public async Task<IActionResult> GetMedicalHistoryAsync([FromRoute] int patientId)
        {
            var result = await patientService.GetMedicalHistoryAsync(patientId);

            if (!result.Success)
            {
                return HandleFailure(result);
            }

            return Ok(result.Data);
        }
    }
}