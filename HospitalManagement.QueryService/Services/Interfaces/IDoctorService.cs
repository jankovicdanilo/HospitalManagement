using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Models.DTOs.Doctor;

namespace HospitalManagement.QueryService.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<Result<PagedResult<DoctorResponseDto>>> GetAllAsync(int pageNumber, int pageSize);
        Task<Result<DoctorResponseDto>> GetByIdAsync(int id);
    }
}