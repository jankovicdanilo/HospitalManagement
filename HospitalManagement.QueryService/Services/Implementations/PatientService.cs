using AutoMapper;
using HospitalManagement.QueryService.Models.DTOs.Patient;
using HospitalManagement.QueryService.Repositories.Interfaces;
using HospitalManagement.QueryService.Services.Interfaces;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.QueryService.Services.Implementations
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

        public async Task<Result<List<PatientListDto>>> GetAllAsync()
        {
            var patients = await patientRepository.GetAllAsync();
            var result = mapper.Map<List<PatientListDto>>(patients);
            return Result<List<PatientListDto>>.Ok(result);
        }

        public async Task<Result<PatientGetByIdDto?>> GetByIdAsync(int id)
        {
            var patient = await patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                logger.LogWarning("Patient with id {Id} not found", id);
                return Result<PatientGetByIdDto?>.Fail($"Patient with the id {id} doesn't exist", "INVALID_ID");
            }
            var result = mapper.Map<PatientGetByIdDto>(patient);
            return Result<PatientGetByIdDto?>.Ok(result);
        }

        public async Task<Result<PatientMedicalHistoryDto>> GetMedicalHistoryAsync(int patientId)
        {
            // TODO: cross-service HTTP call to appointment microservice
            await Task.CompletedTask;
            return Result<PatientMedicalHistoryDto>.Fail(
                "Medical history temporarily unavailable — pending cross-service implementation",
                "NOT_IMPLEMENTED");
        }
    }
}