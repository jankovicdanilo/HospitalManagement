using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
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

        public async Task<Result<List<PatientListResponseDto>>> GetAllAsync()
        {
            var patientsDomain = await patientRepository.GetAllAsync();

            var result = new List<PatientListResponseDto>();

            foreach(var patient in patientsDomain)
            {
                result.Add(new PatientListResponseDto
                (
                    patient.Id,
                    patient.Name,
                    patient.LastName,
                    patient.DateOfBirth,
                    patient.Email,
                    patient.Phone
                ));
            }

            return Result<List<PatientListResponseDto>>.Ok(result);
        }

        public async Task<Result<PatientResponseDto?>> GetByIdAsync(int id)
        {
            var patientDomain = await patientRepository.GetByIdAsync(id);

            if(patientDomain == null)
            {
                return Result<PatientResponseDto?>.Fail($"Patient with the id {id} not found", "INVALID_ID", 
                    ErrorType.NotFound);
            }

            var result = new PatientResponseDto
                (
                    patientDomain.Id,
                    patientDomain.Name,
                    patientDomain.LastName,
                    patientDomain.DateOfBirth,
                    patientDomain.Email,
                    patientDomain.Phone
                );

            return Result<PatientResponseDto?>.Ok(result);
        }
    }
}
