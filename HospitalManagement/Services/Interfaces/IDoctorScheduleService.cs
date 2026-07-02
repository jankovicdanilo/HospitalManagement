using HospitalManagement.Shared.Models.DTOs.DoctorSchedule;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Models.DTOs;

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
