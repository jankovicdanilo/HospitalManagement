using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;

namespace HospitalManagement.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository patientRepository;
        
        public PatientService(IPatientRepository patientRepository)
        {
            this.patientRepository = patientRepository;
        }
    }
}
