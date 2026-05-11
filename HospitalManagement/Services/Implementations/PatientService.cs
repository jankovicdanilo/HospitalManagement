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

        public async Task<Result<PatientUpdateResponseDto>> UpdateAsync(PatientUpdateRequestDto request)
        {
            var patientDomain = await patientRepository.GetByIdAsync(request.Id);

            if (patientDomain == null)
            {
                return Result<PatientUpdateResponseDto>.Fail($"Patient with the id {request.Id} not found", "INVALID_ID");
            }

            if (patientRepository.EmailExists(request.Email) && request.Email != patientDomain.Email)
            {
                return Result<PatientUpdateResponseDto>.Fail($"Email {request.Email} already exists", "INVALID_EMAIL");
            }

            patientDomain.Id = request.Id;
            patientDomain.Name = request.Name;
            patientDomain.LastName = request.LastName;
            patientDomain.Email = request.Email;
            patientDomain.DateOfBirth = request.DateOfBirth;
            patientDomain.Phone = request.Phone;


            await patientRepository.UpdateAsync(patientDomain);

            var result = new PatientUpdateResponseDto
                (
                    patientDomain.Id,
                    patientDomain.Name,
                    patientDomain.LastName,
                    patientDomain.DateOfBirth,
                    patientDomain.Email,
                    patientDomain.Phone
                );

            return Result<PatientUpdateResponseDto>.Ok(result);
        }
    }
}
