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

        public async Task<Result<CreatePatientResponseDto?>> CreateAsync(CreatePatientRequestDto request)
        {
            if (request == null)
            {
                return Result<CreatePatientResponseDto?>.Fail("Patient not found", "PATIENT_NOT_FOUND");
            }

            var patientExists = await patientRepository.GetByEmail(request.Email);

            if (patientExists != null)
            {
                return Result<CreatePatientResponseDto?>.Fail($"Email {request.Email} aldready exists", "INVALID_EMAIL");
            }

            var patientDomain = new Patient
            {
                Name = request.Name,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Email = request.Email,
                Phone = request.Phone
            };

            patientDomain = await patientRepository.CreateAsync(patientDomain);

            var result = new CreatePatientResponseDto
                (
                    patientDomain.Id,
                    patientDomain.Name,
                    patientDomain.LastName,
                    patientDomain.DateOfBirth,
                    patientDomain.Email,
                    patientDomain.Phone
                );

            return Result<CreatePatientResponseDto?>.Ok(result);
        }
    }
}
