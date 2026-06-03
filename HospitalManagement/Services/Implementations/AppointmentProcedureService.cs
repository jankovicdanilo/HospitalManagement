using AutoMapper;
using Azure.Core;
using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.AppointmentProcedure;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;

namespace HospitalManagement.Services.Implementations
{
    public class AppointmentProcedureService : IAppointmentProcedureService
    {
        private readonly IAppointmentProcedureRepository appointmentProcedureRepository;
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IProcedureRepository procedureRepository;
        private readonly IMapper mapper;

        public AppointmentProcedureService(IAppointmentProcedureRepository appointmentProcedureRepository,
            IAppointmentRepository appointmentRepository, IProcedureRepository procedureRepository,
            IMapper mapper)
        {
            this.appointmentProcedureRepository = appointmentProcedureRepository;
            this.procedureRepository = procedureRepository;
            this.appointmentRepository = appointmentRepository;
            this.mapper = mapper;
        }

        public async Task<Result<AppointmentProcedureCreateResponseDto>> AddAsync(AppointmentProcedureCreateRequestDto request)
        {
            var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);

            if (appointment == null)
            {
                return Result<AppointmentProcedureCreateResponseDto>.Fail($"Appointment with id {request.AppointmentId} not found", "INVALID_APPOINTMENT_ID");
            }

            return null;
        }

        public async Task<Result<AppointmentProcedureResponseDto>> GetAsync(int appointmentId, int procedureId)
        {
            var appointment = await appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                return Result<AppointmentProcedureResponseDto>.Fail($"Appointment with id {appointmentId} not found", "INVALID_ID");
            }
                

            var procedure = await procedureRepository.GetByIdAsync(procedureId);
            if (procedure == null)
            {
                return Result<AppointmentProcedureResponseDto>.Fail($"Procedure with id {procedureId} not found", "INVALID_ID");
            }
                
            var appointmentProcedureDomain = await appointmentProcedureRepository.GetAsync(appointmentId, procedureId);

            if(appointmentProcedureDomain == null)
            {
                return Result<AppointmentProcedureResponseDto>.Fail($"Procedure {procedureId} is not linked to appointment {appointmentId}", "INVALID_ID");
            }

            var result = mapper.Map<AppointmentProcedureResponseDto>(appointmentProcedureDomain);

            return Result<AppointmentProcedureResponseDto>.Ok(result);
        }

        public Task<Result<AppointmentProcedureResponseDto>> RemoveAsync(int appointmentId, int procedureId)
        {
            throw new NotImplementedException();
        }
    }
}
