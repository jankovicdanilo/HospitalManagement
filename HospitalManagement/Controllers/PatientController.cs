using HospitalManagement.Services.Implementations;
using HospitalManagement.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService patientService;

        public PatientController(IPatientService patientService)
        {
            this.patientService = patientService;
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await patientService.Delete(id);

            return Ok(result);
        }
    }
}
