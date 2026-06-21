using HospitalManagement.CommandService.Models.Domain;

namespace HospitalManagement.CommandService.Repositories.Interfaces
{
    public interface IDoctorRepository
    {
        Task<Doctor?> CreateAsync(Doctor request);
        Task<Doctor?> UpdateAsync(Doctor request);
        Task Delete(int id);
        Task<Doctor?> GetByIdAsync(int id);
    }
}