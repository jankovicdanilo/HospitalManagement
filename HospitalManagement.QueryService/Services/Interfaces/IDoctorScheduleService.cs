using HospitalManagement.QueryService.Models.DTOs.DoctorSchedule;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.QueryService.Services.Interfaces
{
    public interface IDoctorScheduleService
    {
        Task<Result<DoctorScheduleResponseDto>> GetByIdAsync(int id);
        Task<Result<List<DoctorScheduleResponseDto>>> GetAllByDoctorIdAsync(int doctorId);
        Task<Result<DoctorScheduleResponseDto>> GetByDoctorIdAndDayAsync(int doctorId, DayOfWeek dayOfWeek);
    }
}