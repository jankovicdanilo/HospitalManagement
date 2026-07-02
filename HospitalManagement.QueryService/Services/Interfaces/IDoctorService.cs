using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Models.DTOs.Doctor;

namespace HospitalManagement.QueryService.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<Result<List<DoctorResponseDto>>> GetAllAsync();
        Task<Result<DoctorResponseDto>> GetByIdAsync(int id);
    }
}