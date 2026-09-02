using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Models.DTOs.Doctor;

namespace HospitalManagement.QueryService.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<Result<PagedResult<DoctorResponseDto>>> GetAllAsync(DoctorFilterDto filter);
        Task<Result<DoctorResponseDto>> GetByIdAsync(int id);
    }
}