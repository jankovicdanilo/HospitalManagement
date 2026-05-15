using AutoMapper;
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
        private readonly IMapper mapper;

        public PatientService(IPatientRepository patientRepository, IMapper mapper)
        {
            this.patientRepository = patientRepository;
            this.mapper = mapper;
        }

        public async Task<Result<List<PatientListDto>>> GetAllAsync()
        {
            var patientsListDomain = await patientRepository.GetAllAsync();

            var result = mapper.Map<List<PatientListDto>>(patientsListDomain);

            return Result<List<PatientListDto>>.Ok(result);
        }

        public async Task<Result<PatientGetByIdDto?>> GetByIdAsync(int id)
        {
            var patientDomain = await patientRepository.GetByIdAsync(id);

            if(patientDomain == null)
            {
                return Result<PatientGetByIdDto?>.Fail($"Patient with the id {id} doesn't exist", "INVALID_ID");
            }

            var result = mapper.Map<PatientGetByIdDto>(patientDomain);

            return Result<PatientGetByIdDto?>.Ok(result);
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

            var patientDomain = mapper.Map<Patient>(request);

            patientDomain = await patientRepository.CreateAsync(patientDomain);

            var result = mapper.Map<CreatePatientResponseDto>(patientDomain);

            return Result<CreatePatientResponseDto?>.Ok(result);
        }
    }
}
