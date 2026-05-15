using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Appointment;

namespace HospitalManagement.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<Result<List<AppointmentListResponseDto>>> GetAllAsync();

        Task<Result<AppointmentResponseDto>> GetByIdAsync(int id);
        Task<Result<AppointmentUpdateResponseDto>> UpdateAsync(AppointmentUpdateRequestDto request);
    }
}
