using AutoMapper;
using HospitalManagement.Shared.Common;
using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Models.DTOs.Treatment;
using HospitalManagement.Appointments.Repositories.Interfaces;
using HospitalManagement.Appointments.Services.Interfaces;
using HospitalManagement.Appointments.Services.Validations;

namespace HospitalManagement.Appointments.Services.Implementations
{
    public class TreatmentService : ITreatmentService
    {
        private readonly ITreatmentRepository treatmentRepository;
        private readonly ITreatmentValidation treatmentValidation;
        private readonly IAppointmentService appointmentService;
        private readonly IMapper mapper;
        private readonly ILogger<TreatmentService> logger;

        public TreatmentService(ITreatmentRepository treatmentRepository,IAppointmentService appointmentService, 
            IMapper mapper, ITreatmentValidation treatmentValidation,
            ILogger<TreatmentService> logger)
        {
            this.treatmentRepository = treatmentRepository;
            this.appointmentService = appointmentService;
            this.mapper = mapper;
            this.treatmentValidation = treatmentValidation;
            this.logger = logger;
        }

        public async Task<Result<TreatmentCreateResponseDto>> CreateAsync(TreatmentCreateRequestDto request)
        {
            var validate = await treatmentValidation.ValidateAll(request);

            if (!validate.Success)
            {
                logger.LogWarning("Treatment creation failed {Message}", validate.Message);
                return Result<TreatmentCreateResponseDto>.Fail(validate.Message, validate.ErrorCode, validate.ErrorType);
            }

            var treatmentDomain = mapper.Map<Treatment>(request);

            treatmentDomain = await treatmentRepository.CreateAsync(treatmentDomain);

            logger.LogInformation("Treatment created with id {id}", treatmentDomain.Id);

            var appointment = await appointmentService.GetByIdAsync(treatmentDomain.AppointmentId);
            if(appointment.Success && appointment.Data?.Patient != null)
            {
                await appointmentService.InvalidatePatientSummaryCacheAsync(appointment.Data.Patient.Id);
            }

            var result = mapper.Map<TreatmentCreateResponseDto>(treatmentDomain);

            return Result<TreatmentCreateResponseDto>.Ok(result);
        }
    }
}