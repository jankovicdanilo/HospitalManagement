using HospitalManagement.QueryService.Models.ReadModels;

namespace HospitalManagement.QueryService.Repositories.Interfaces
{
    public interface IDoctorRepository
    {
        Task<List<DoctorReadModel>> GetAllAsync();
        Task<DoctorReadModel?> GetByIdAsync(int id);
    }
}