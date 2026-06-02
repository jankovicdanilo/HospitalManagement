using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Appointment;

namespace HospitalManagement.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<Result> Delete(int id);
        Task<Result<PagedResult<AppointmentListResponseDto>>> GetAllAsync(AppointmentFilterDto filter);

        Task<Result<AppointmentResponseDto>> GetByIdAsync(int id);

        Task<Result<AppointmentCreateResponseDto>> CreateAsync(AppointmentCreateRequestDto request);

        Task<Result<AppointmentUpdateResponseDto>> UpdateAsync(AppointmentUpdateRequestDto request);

        Task<Result<List<TimeSlotDto>>> GetFreeSlotsAsync(int doctorId, DateOnly date);

        Task<Result> UpdateStatusAsync(AppointmentStatusUpdateDto request);
    }
}
