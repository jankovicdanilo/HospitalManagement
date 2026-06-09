using HospitalManagement.Shared.Common;
using HospitalManagement.Models.DTOs.DoctorSchedule;

namespace HospitalManagement.Services.Interfaces
{
    public interface IDoctorScheduleService
    {
        Task<Result<DoctorScheduleCreateResponseDto>> CreateAsync(DoctorScheduleCreateRequestDto request);
        Task<Result<DoctorScheduleUpdateResponseDto>> UpdateAsync(DoctorScheduleUpdateRequestDto request);
        Task<Result<DoctorScheduleResponseDto>> GetByIdAsync(int id);
        Task<Result<List<DoctorScheduleResponseDto>>> GetAllByDoctorIdAsync(int doctorId);
        Task<Result> Delete(int id);

        Task<Result<DoctorScheduleResponseDto>> GetByDoctorIdAndDayAsync(int doctorId, DayOfWeek dayOfWeek);

    }
}
