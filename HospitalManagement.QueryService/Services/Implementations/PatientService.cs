using AutoMapper;
using HospitalManagement.QueryService.Clients.Interfaces;
using HospitalManagement.QueryService.Models.DTOs.Patient;
using HospitalManagement.QueryService.Repositories.Interfaces;
using HospitalManagement.QueryService.Services.Interfaces;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Models.DTOs.Patient;

namespace HospitalManagement.QueryService.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository patientRepository;
        private readonly IMapper mapper;
        private readonly ILogger<PatientService> logger;
        private readonly IAppointmentServiceClient appointmentServiceClient;

        public PatientService(IPatientRepository patientRepository, IMapper mapper, 
            ILogger<PatientService> logger, IAppointmentServiceClient appointmentServiceClient)
        {
            this.patientRepository = patientRepository;
            this.mapper = mapper;
            this.logger = logger;
            this.appointmentServiceClient = appointmentServiceClient;
        }

        public async Task<Result<PagedResult<PatientListDto>>> GetAllAsync(PatientFilterDto filter)
        {
            var (patients, totalCount) = await patientRepository.GetAllAsync(filter);
            var mapped = mapper.Map<List<PatientListDto>>(patients);

            var pagedResult = new PagedResult<PatientListDto>
            {
                Items = mapped,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            return Result<PagedResult<PatientListDto>>.Ok(pagedResult);
        }

        public async Task<Result<PatientGetByIdDto?>> GetByIdAsync(int id)
        {
            var patient = await patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                logger.LogWarning("Patient with id {Id} not found", id);
                return Result<PatientGetByIdDto?>.Fail($"Patient with the id {id} doesn't exist", "INVALID_ID", 
                    ErrorType.NotFound);
            }

            var result = mapper.Map<PatientGetByIdDto>(patient);
            return Result<PatientGetByIdDto?>.Ok(result);
        }

        public async Task<Result<PatientMedicalHistoryDto>> GetMedicalHistoryAsync(int patientId)
        {
            var patient = await patientRepository.GetByIdAsync(patientId);

            if(patient == null)
            {
                logger.LogWarning("Patient with id {PatientId} not found", patientId);
                return Result<PatientMedicalHistoryDto>.Fail(
                    $"Patient with id {patientId} not found", "INVALID_PATIENT_ID", ErrorType.NotFound);
            }

            var patientMedicalHistory = await appointmentServiceClient.GetPatientHistoryAsync(patientId);

            if(patientMedicalHistory == null)
            {
                logger.LogWarning("Could not retrieve history for patient {PatientId}", patientId);
                return Result<PatientMedicalHistoryDto>.Fail(
                    "Could not retrieve patient history", "HISTORY_UNAVAILABLE", ErrorType.UpstreamFailure);
            }

            patientMedicalHistory.PatientName = $"{patient.Name} {patient.LastName}";

            return Result<PatientMedicalHistoryDto>.Ok(patientMedicalHistory);
        }
    }
}