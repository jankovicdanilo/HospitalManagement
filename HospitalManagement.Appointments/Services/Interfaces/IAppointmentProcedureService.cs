using HospitalManagement.Shared.Common;
using HospitalManagement.Appointments.Models.DTOs.AppointmentProcedure;

namespace HospitalManagement.Appointments.Services.Interfaces
{
    public interface IAppointmentProcedureService
    {
        Task<Result<AppointmentProcedureResponseDto>> GetByAppointmentAndProcedureIdAsync(int appointmentId, int procedureId);
        Task<Result<AppointmentProcedureCreateResponseDto>> CreateAsync(AppointmentProcedureCreateRequestDto request);
        Task<Result<AppointmentProcedureResponseDto>> DeleteAsync(int appointmentId, int procedureId);
    }
}