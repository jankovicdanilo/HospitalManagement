using HospitalManagement.Common;
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

        public async Task<Result> Delete(int id)
        {
            await patientRepository.Delete(id);

            return Result.Ok($"Patient with id {id} deleted");
        }
    }
}
