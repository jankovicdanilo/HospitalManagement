using HospitalManagement.Shared.Models.Domain;

namespace HospitalManagement.QueryService.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task<List<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task<Patient?> GetByEmailAsync(string email);
    }
}