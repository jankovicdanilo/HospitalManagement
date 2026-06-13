using HospitalManagement.Shared.Common;
using HospitalManagement.Models.DTOs.Doctor;

namespace HospitalManagement.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<Result<DoctorResponseDto>> CreateAsync(DoctorCreateRequestDto request);

        Task<Result<List<DoctorResponseDto>>> GetAllAsync();

        Task<Result<DoctorResponseDto>> GetByIdAsync(int id);

        Task<Result<DoctorResponseDto>> UpdateAsync(DoctorUpdateRequestDto request);

        Task<Result> Delete(int id);
    }
}
