using HospitalManagement.Shared.Models.DTOs.Doctor;
using HospitalManagement.Shared.Models.DTOs.Doctor;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.CommandService.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<Result<DoctorResponseDto>> CreateAsync(DoctorCreateRequestDto request);
        Task<Result<DoctorResponseDto>> UpdateAsync(DoctorUpdateRequestDto request);
        Task<Result> Delete(int id);
    }
}