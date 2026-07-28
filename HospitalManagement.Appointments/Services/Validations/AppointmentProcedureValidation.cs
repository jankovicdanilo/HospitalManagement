using HospitalManagement.Shared.Common;
using HospitalManagement.Appointments.Models.Enums;
using HospitalManagement.Appointments.Repositories.Interfaces;
using HospitalManagement.Appointments.Clients.Interfaces;

namespace HospitalManagement.Appointments.Services.Validations
{
    public class AppointmentProcedureValidation : IAppointmentProcedureValidation
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IAppointmentProcedureRepository appointmentProcedureRepository;
        private readonly IQueryServiceClient hospitalClient;

        public AppointmentProcedureValidation(IAppointmentRepository appointmentRepository,
            IAppointmentProcedureRepository appointmentProcedureRepository,
            IQueryServiceClient hospitalClient)
        {
            this.appointmentRepository = appointmentRepository;
            this.appointmentProcedureRepository = appointmentProcedureRepository;
            this.hospitalClient = hospitalClient;
        }

        public async Task<Result> ValidateForCreate(int appointmentId, int procedureId)
        {
            var appointment = await appointmentRepository.GetByIdAsync(appointmentId);

            if (appointment == null)
            {
                return Result.Fail($"Appointment with id {appointmentId} not found", "INVALID_APPOINTMENT_ID",
                    ErrorType.NotFound);
            }

            if (appointment.Status != AppointmentStatus.Pending)
            {
                return Result.Fail("Procedures can only be modified on Pending appointments", "INVALID_STATUS",
                    ErrorType.Conflict);
            }

            var procedure = await hospitalClient.GetProcedureAsync(procedureId);
            if (procedure == null)
            {
                return Result.Fail($"Procedure with id {procedureId} not found", "INVALID_PROCEDURE_ID",
                    ErrorType.NotFound);
            }

            if (await IsDuplicate(appointmentId, procedureId))
            {
                return Result.Fail($"Procedure {procedureId} is already linked to appointment {appointmentId}", "DUPLICATE",
                    ErrorType.Conflict);
            }

            return Result.Ok("Validation ok");
        }

        public async Task<Result> ValidateForGet(int appointmentId, int procedureId)
        {
            var appointment = await appointmentRepository.GetByIdAsync(appointmentId);

            if (appointment == null)
            {
                return Result.Fail($"Appointment with id {appointmentId} not found", "INVALID_APPOINTMENT_ID",
                    ErrorType.NotFound);
            }

            var procedure = await hospitalClient.GetProcedureAsync(procedureId);
            if (procedure == null)
            {
                return Result.Fail($"Procedure with id {procedureId} not found", "INVALID_PROCEDURE_ID", ErrorType.NotFound);
            }

            return Result.Ok("Validation ok");
        }

        public async Task<Result> ValidateForDelete(int appointmentId, int procedureId)
        {
            var appointment = await appointmentRepository.GetByIdAsync(appointmentId);

            if (appointment == null)
            {
                return Result.Fail($"Appointment with id {appointmentId} not found", "INVALID_APPOINTMENT_ID", 
                    ErrorType.NotFound);
            }

            var procedure = await hospitalClient.GetProcedureAsync(procedureId);
            if (procedure == null)
            {
                return Result.Fail($"Procedure with id {procedureId} not found", "INVALID_PROCEDURE_ID", 
                    ErrorType.NotFound);
            }

            var link = await appointmentProcedureRepository.GetByAppointmentAndProcedureIdAsync(appointmentId, procedureId);

            if (link == null)
            {
                return Result.Fail($"Procedure {procedureId} is not linked to appointment {appointmentId}",
                    "PROCEDURE_NOT_LINKED", ErrorType.NotFound);
            }

            if (appointment.Status != AppointmentStatus.Pending)
            {
                return Result.Fail("Procedures can only be modified on Pending appointments", "INVALID_STATUS",
                    ErrorType.Conflict);
            }

            return Result.Ok("Validation ok");
        }

        private async Task<bool> IsDuplicate(int appointmentId, int procedureId)
        {
            return await appointmentProcedureRepository.GetByAppointmentAndProcedureIdAsync(appointmentId, procedureId) != null;
        }
    }
}