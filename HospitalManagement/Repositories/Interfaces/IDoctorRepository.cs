

using HospitalManagement.Models.Domain;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface IDoctorRepository
    {
        Task<Doctor?> CreateAsync(Doctor request);

        Task<List<Doctor>> GetAllAsync();

        Task<Doctor?> GetByIdAsync(int id);

        Task<Doctor?> UpdateAsync(Doctor request);

        Task Delete(int id);
    }
}
