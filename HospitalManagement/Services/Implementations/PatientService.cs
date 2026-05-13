using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Patient;
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

        public async Task<Result<List<PatientListDto>>> GetAllAsync()
        {
            var patientsListDomain = await patientRepository.GetAllAsync();

            List<PatientListDto> result = new List<PatientListDto>();

            foreach (var patient in patientsListDomain)
            {
                result.Add(new PatientListDto
                    (
                        patient.Id,
                        patient.Name,
                        patient.DateOfBirth,
                        patient.Email,
                        patient.Phone,
                        patient.LastName
                    ));
            }

            return Result<List<PatientListDto>>.Ok(result);
        }

        public async Task<Result<PatientGetByIdDto?>> GetByIdAsync(int id)
        {
            var patientDomain = await patientRepository.GetByIdAsync(id);

            if(patientDomain == null)
            {
                return Result<PatientGetByIdDto?>.Fail($"Patient with the id {id} doesn't exist", "INVALID_ID");
            }

            var result = new PatientGetByIdDto
                (
                    patientDomain.Id,
                    patientDomain.Name,
                    patientDomain.DateOfBirth,
                    patientDomain.Email,
                    patientDomain.LastName,
                    patientDomain.Phone
                );

            return Result<PatientGetByIdDto?>.Ok(result);
        }
    }
}
