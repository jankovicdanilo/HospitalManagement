using HospitalManagement.Shared.Common;
using HospitalManagement.Appointments.Models.DTOs.Appointment;

namespace HospitalManagement.Appointments.Services.Interfaces
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
        Task<Result<List<AppointmentResponseDto>>> GetPatientHistoryAsync(int patientId);
        Task<Result<List<int>>> GetPopularDoctorIdsAsync(int count);
        Task<Result<List<int>>> GetPopularPatientIdsAsync(int count);
    }
}