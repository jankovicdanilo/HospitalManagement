using AutoMapper;
using Azure.Core;
using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Models.DTOs.AppointmentProcedure;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;
using HospitalManagement.Services.Validations;
using NLog;

namespace HospitalManagement.Services.Implementations
{
    public class AppointmentProcedureService : IAppointmentProcedureService
    {
        private readonly IAppointmentProcedureRepository appointmentProcedureRepository;
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IProcedureRepository procedureRepository;
        private readonly IMapper mapper;
        private readonly IAppointmentProcedureValidation appointmentProcedureValidation;
        private readonly ILogger<AppointmentProcedureService> logger;

        public AppointmentProcedureService(IAppointmentProcedureRepository appointmentProcedureRepository,
            IAppointmentRepository appointmentRepository, IProcedureRepository procedureRepository,
            IMapper mapper, IAppointmentProcedureValidation appointmentProcedureValidation, ILogger<AppointmentProcedureService> logger)
        {
            this.appointmentProcedureRepository = appointmentProcedureRepository;
            this.procedureRepository = procedureRepository;
            this.appointmentRepository = appointmentRepository;
            this.mapper = mapper;
            this.appointmentProcedureValidation = appointmentProcedureValidation;
            this.logger = logger;
        }

        public async Task<Result<AppointmentProcedureCreateResponseDto>> CreateAsync(AppointmentProcedureCreateRequestDto request)
        {
            var validatedAppointmentProcedure = await appointmentProcedureValidation.ValidateForCreate(request.AppointmentId, request.ProcedureId);

            if (!validatedAppointmentProcedure.Success)
            {
                logger.LogWarning("{Message}", validatedAppointmentProcedure.Message);
                return Result<AppointmentProcedureCreateResponseDto>.Fail(validatedAppointmentProcedure.Message, validatedAppointmentProcedure.ErrorCode);
            }

            var appointmentProcedureDomain = mapper.Map<AppointmentProcedure>(request);

            appointmentProcedureDomain = await appointmentProcedureRepository.CreateAsync(appointmentProcedureDomain);

            logger.LogInformation("Appointment procedure created with appointment id {AppointmentId} " +
                "and procedure id {ProcedureId}", 
                appointmentProcedureDomain.AppointmentId, appointmentProcedureDomain.ProcedureId);

            var result = mapper.Map<AppointmentProcedureCreateResponseDto>(appointmentProcedureDomain);

            return Result<AppointmentProcedureCreateResponseDto>.Ok(result);
        }

        public async Task<Result<AppointmentProcedureResponseDto>> GetByAppointmentAndProcedureIdAsync(int appointmentId, int procedureId)
        {
            var validatedAppointmentProcedure = await appointmentProcedureValidation.ValidateForGet(appointmentId, procedureId);

            if (!validatedAppointmentProcedure.Success)
            {
                logger.LogWarning("{Message}", validatedAppointmentProcedure.Message);
                return Result<AppointmentProcedureResponseDto>.Fail(validatedAppointmentProcedure.Message, validatedAppointmentProcedure.ErrorCode);
            }

            var appointmentProcedureDomain = await appointmentProcedureRepository.GetByAppointmentAndProcedureIdAsync(appointmentId, procedureId);

            if(appointmentProcedureDomain == null)
            {
                logger.LogWarning("Appointment {appointmentId} is not linked to Procedure {procedureId}", appointmentId, procedureId);
                return Result<AppointmentProcedureResponseDto>.Fail($"Appointment {appointmentId} is not linked to Procedure {procedureId}", "INVALID_ID");
            }

            var result = mapper.Map<AppointmentProcedureResponseDto>(appointmentProcedureDomain);

            return Result<AppointmentProcedureResponseDto>.Ok(result);
        }

        public async Task<Result<AppointmentProcedureResponseDto>> DeleteAsync(int appointmentId, int procedureId)
        {
            var validatedAppointmentProcedure = await appointmentProcedureValidation.ValidateForDelete(appointmentId, procedureId);

            if (!validatedAppointmentProcedure.Success)
            {
                logger.LogWarning("{Message}", validatedAppointmentProcedure.Message);
                return Result<AppointmentProcedureResponseDto>.Fail(validatedAppointmentProcedure.Message, validatedAppointmentProcedure.ErrorCode);
            }

            var appointmentProcedureDomain = await appointmentProcedureRepository.DeleteAsync(appointmentId, procedureId);

            if(appointmentProcedureDomain == null)
            {
                logger.LogWarning("Appointment {appointmentId} is not linked to Procedure {procedureId}", appointmentId, procedureId);
                return Result<AppointmentProcedureResponseDto>.Fail($"Appointment {appointmentId} is not linked to Procedure {procedureId}", "INVALID_ID");
            }

            logger.LogInformation("Procedure {ProcedureId} removed from Appointment {AppointmentId}", procedureId, appointmentId);

            var result = mapper.Map<AppointmentProcedureResponseDto>(appointmentProcedureDomain);

            return Result<AppointmentProcedureResponseDto>.Ok(result);
        }
    }
}
