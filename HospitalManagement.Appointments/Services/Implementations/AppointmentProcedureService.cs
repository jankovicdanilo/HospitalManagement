using AutoMapper;
using HospitalManagement.Appointments.Clients.Interfaces;
using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Models.DTOs.Appointment;
using HospitalManagement.Appointments.Models.DTOs.AppointmentProcedure;
using HospitalManagement.Appointments.Repositories.Interfaces;
using HospitalManagement.Appointments.Services.Interfaces;
using HospitalManagement.Appointments.Services.Validations;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.Appointments.Services.Implementations
{
    public class AppointmentProcedureService : IAppointmentProcedureService
    {
        private readonly IAppointmentProcedureRepository appointmentProcedureRepository;
        private readonly IMapper mapper;
        private readonly IAppointmentProcedureValidation appointmentProcedureValidation;
        private readonly ILogger<AppointmentProcedureService> logger;
        private readonly IQueryServiceClient hospitalManagementClient;

        public AppointmentProcedureService(IAppointmentProcedureRepository appointmentProcedureRepository,
            IMapper mapper, IAppointmentProcedureValidation appointmentProcedureValidation, 
            ILogger<AppointmentProcedureService> logger, IQueryServiceClient hospitalManagementClient)
        {
            this.appointmentProcedureRepository = appointmentProcedureRepository;
            this.mapper = mapper;
            this.appointmentProcedureValidation = appointmentProcedureValidation;
            this.logger = logger;
            this.hospitalManagementClient = hospitalManagementClient;
        }

        public async Task<Result<AppointmentProcedureCreateResponseDto>> CreateAsync
            (AppointmentProcedureCreateRequestDto request)
        {
            var validatedAppointmentProcedure = await appointmentProcedureValidation.ValidateForCreate(request.AppointmentId, request.ProcedureId);

            if (!validatedAppointmentProcedure.Success)
            {
                logger.LogWarning("{Message}", validatedAppointmentProcedure.Message);
                return Result<AppointmentProcedureCreateResponseDto>.Fail(validatedAppointmentProcedure.Message, 
                    validatedAppointmentProcedure.ErrorCode, validatedAppointmentProcedure.ErrorType);
            }

            var procedure = await hospitalManagementClient.GetProcedureAsync(request.ProcedureId);
            if(procedure == null)
            {
                logger.LogWarning("Procedure with id {ProcedureId} not found", request.ProcedureId);
                return Result<AppointmentProcedureCreateResponseDto>.Fail($"Procedure with id {request.ProcedureId} not found",
                    "INVALID_PROCEDURE_ID", ErrorType.NotFound);
            }

            var appointmentProcedureDomain = mapper.Map<AppointmentProcedure>(request);
            appointmentProcedureDomain.ProcedureName = procedure.Name;
            appointmentProcedureDomain.ProcedurePrice = procedure.Price;

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
                return Result<AppointmentProcedureResponseDto>.Fail(validatedAppointmentProcedure.Message, 
                    validatedAppointmentProcedure.ErrorCode, validatedAppointmentProcedure.ErrorType);
            }

            var appointmentProcedureDomain = await appointmentProcedureRepository.GetByAppointmentAndProcedureIdAsync(appointmentId, procedureId);

            if (appointmentProcedureDomain == null)
            {
                logger.LogWarning("Appointment {appointmentId} is not linked to Procedure {procedureId}", appointmentId, procedureId);
                return Result<AppointmentProcedureResponseDto>.Fail($"Appointment {appointmentId} is not linked to Procedure " +
                    $"{procedureId}", "INVALID_ID", ErrorType.NotFound);
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
                return Result<AppointmentProcedureResponseDto>.Fail(validatedAppointmentProcedure.Message, 
                    validatedAppointmentProcedure.ErrorCode, validatedAppointmentProcedure.ErrorType);
            }

            var appointmentProcedureDomain = await appointmentProcedureRepository.DeleteAsync(appointmentId, procedureId);

            if (appointmentProcedureDomain == null)
            {
                logger.LogWarning("Appointment {appointmentId} is not linked to Procedure {procedureId}", appointmentId, procedureId);
                return Result<AppointmentProcedureResponseDto>.Fail($"Appointment {appointmentId} is not linked to Procedure " +
                    $"{procedureId}", "INVALID_ID", ErrorType.NotFound);
            }

            logger.LogInformation("Procedure {ProcedureId} removed from Appointment {AppointmentId}", procedureId, appointmentId);

            var result = mapper.Map<AppointmentProcedureResponseDto>(appointmentProcedureDomain);

            return Result<AppointmentProcedureResponseDto>.Ok(result);
        }
    }
}