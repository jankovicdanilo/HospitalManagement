using HospitalManagement.Common;
using HospitalManagement.Models.DTOs;
using HospitalManagement.Models.DTOs.AppointmentProcedure;

namespace HospitalManagement.Services.Interfaces
{
    public interface IAppointmentProcedureService
    {
        Task<Result<AppointmentProcedureResponseDto>> GetByAppointmentAndProcedureIdAsync(int appointmentId, int procedureId);  
        Task<Result<AppointmentProcedureCreateResponseDto>> CreateAsync(AppointmentProcedureCreateRequestDto request);
        Task<Result<AppointmentProcedureResponseDto>> DeleteAsync(int appointmentId, int procedureId);
    }
}
