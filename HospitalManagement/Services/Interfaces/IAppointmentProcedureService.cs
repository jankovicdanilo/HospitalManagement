using HospitalManagement.Common;
using HospitalManagement.Models.DTOs;
using HospitalManagement.Models.DTOs.AppointmentProcedure;

namespace HospitalManagement.Services.Interfaces
{
    public interface IAppointmentProcedureService
    {
        Task<Result<AppointmentProcedureResponseDto>> GetAsync(int appointmentId, int procedureId);  
        Task<Result<AppointmentProcedureCreateResponseDto>> AddAsync(AppointmentProcedureCreateRequestDto request);
        Task<Result<AppointmentProcedureResponseDto>> RemoveAsync(int appointmentId, int procedureId);
    }
}
