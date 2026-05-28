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
        private readonly ILogger<PatientService> logger;

        public PatientService(IPatientRepository patientRepository, IMapper mapper, ILogger<PatientService> logger)
        {
            this.patientRepository = patientRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<Result> Delete(int id)
        {
            var patientDomain = await patientRepository.Delete(id);

            if (patientDomain == null)
            {
                logger.LogWarning("Patient with id {Id} not found for deletion", id);
                return Result.Fail($"Patient with the id {id} not found", "INVALID_ID");
            }

            logger.LogInformation("Patient with id {Id} deleted", id);

            return Result.Ok($"Patient with id {id} deleted"); 
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
                logger.LogWarning("Patient with id {Id} not found", id);
                return Result<PatientGetByIdDto?>.Fail($"Patient with the id {id} doesn't exist", "INVALID_ID");
            }

            var result = mapper.Map<PatientGetByIdDto>(patientDomain);

            return Result<PatientGetByIdDto?>.Ok(result);
        }

        public async Task<Result<PatientCreateResponseDto?>> CreateAsync(PatientCreateRequestDto request)
        {
            var patientExists = await patientRepository.GetByEmailAsync(request.Email);

            if (patientExists != null)
            {
                logger.LogWarning("Patient creation failed, email {Email} already exists", request.Email);
                return Result<PatientCreateResponseDto?>.Fail($"Email {request.Email} aldready exists", "INVALID_EMAIL");
            }

            var patientDomain = mapper.Map<Patient>(request);

            patientDomain = await patientRepository.CreateAsync(patientDomain);

            logger.LogInformation("Patient created with id {Id}", patientDomain.Id);

            var result = mapper.Map<PatientCreateResponseDto>(patientDomain);

            return Result<PatientCreateResponseDto?>.Ok(result);
        }

        public async Task<Result<PatientUpdateResponseDto>> UpdateAsync(PatientUpdateRequestDto request)
        {
            var patientDomain = await patientRepository.GetByIdAsync(request.Id);

            if (patientDomain == null)
            {
                logger.LogWarning("Patient with id {Id} not found for update", request.Id);
                return Result<PatientUpdateResponseDto>.Fail($"Patient with the id {request.Id} not found", "INVALID_ID");
            }

            if (await patientRepository.EmailExists(request.Email) && request.Email != patientDomain.Email)
            {
                logger.LogWarning("Patient creation failed, email {Email} already exists", request.Email);
                return Result<PatientUpdateResponseDto>.Fail($"Email {request.Email} already exists", "INVALID_EMAIL");
            }

            mapper.Map(request, patientDomain);

            await patientRepository.UpdateAsync(patientDomain);

            logger.LogInformation("Patient with id {Id} updated", patientDomain.Id);

            var result = mapper.Map<PatientUpdateResponseDto>(patientDomain);

            return Result<PatientUpdateResponseDto>.Ok(result);
        }

        public async Task<Result<PatientMedicalHistoryDto>> GetMedicalHistoryAsync(int patientId)
        {
            if(!await patientRepository.PatientExists(patientId))
            {
                logger.LogWarning("Patient with id {Id} not found", patientId);
                return Result<PatientMedicalHistoryDto>.Fail($"Patient with the id {patientId} doesn't exist", "INVALID_ID");
            }

            var result = await patientRepository.GetMedicalHistoryAsync(patientId);

            return Result<PatientMedicalHistoryDto>.Ok(result);
        }
    }
}
