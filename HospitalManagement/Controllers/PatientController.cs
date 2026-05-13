using HospitalManagement.Models.DTOs.Patient;
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

        {

            {
                return NotFound(new {result.Message, result.ErrorCode});
            }

            return Ok(result);
        }
    }
}
