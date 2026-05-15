using HospitalManagement.Models.Domain;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task<List<Patient>> GetAllAsync();

        Task<Patient?> GetByIdAsync(int id);

        Task<Patient?> CreateAsync(Patient patient);

        Task<Patient> GetByEmail(string email);
        Task<Patient?> GetByIdAsync(int id);
    }
}
