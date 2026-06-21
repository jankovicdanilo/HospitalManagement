using HospitalManagement.QueryService.Models.Doctor;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.QueryService.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<Result<List<DoctorResponseDto>>> GetAllAsync();
        Task<Result<DoctorResponseDto>> GetByIdAsync(int id);
    }
}