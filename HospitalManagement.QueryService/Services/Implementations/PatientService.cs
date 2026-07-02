using AutoMapper;
using HospitalManagement.QueryService.Clients.Interfaces;
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
        private readonly IAppointmentServiceClient appointmentServiceClient;

        public PatientService(IPatientRepository patientRepository, IMapper mapper, 
            ILogger<PatientService> logger, IAppointmentServiceClient appointmentServiceClient)
        {
            this.patientRepository = patientRepository;
            this.mapper = mapper;
            this.logger = logger;
            this.appointmentServiceClient = appointmentServiceClient;
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
            var patient = await patientRepository.GetByIdAsync(patientId);

            if(patient == null)
            {
                logger.LogWarning("Patient with id {PatientId} not found", patientId);
                return Result<PatientMedicalHistoryDto>.Fail(
                    $"Patient with id {patientId} not found", "INVALID_PATIENT_ID");
            }

            var patientMedicalHistory = await appointmentServiceClient.GetPatientHistoryAsync(patientId);

            if(patientMedicalHistory == null)
            {
                logger.LogWarning("Could not retrieve history for patient {PatientId}", patientId);
                return Result<PatientMedicalHistoryDto>.Fail(
                    "Could not retrieve patient history", "HISTORY_UNAVAILABLE");
            }

            patientMedicalHistory.PatientName = $"{patient.Name} {patient.LastName}";

            return Result<PatientMedicalHistoryDto>.Ok(patientMedicalHistory);
        }
    }
}