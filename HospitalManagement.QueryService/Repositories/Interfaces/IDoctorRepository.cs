using HospitalManagement.Shared.Models.Domain;

namespace HospitalManagement.QueryService.Repositories.Interfaces
{
    public interface IDoctorRepository
    {
        Task<(List<Doctor> items, int totalCount)> GetAllAsync(int pageNumber, int pageSize);
        Task<Doctor?> GetByIdAsync(int id);
    }
}