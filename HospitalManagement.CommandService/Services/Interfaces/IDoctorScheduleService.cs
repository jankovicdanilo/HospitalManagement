using HospitalManagement.CommandService.Models.DTOs.DoctorSchedule;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.CommandService.Services.Interfaces
{
    public interface IDoctorScheduleService
    {
        Task<Result<DoctorScheduleCreateResponseDto>> CreateAsync(DoctorScheduleCreateRequestDto request);
        Task<Result<DoctorScheduleUpdateResponseDto>> UpdateAsync(DoctorScheduleUpdateRequestDto request);
        Task<Result> Delete(int id);
    }
}