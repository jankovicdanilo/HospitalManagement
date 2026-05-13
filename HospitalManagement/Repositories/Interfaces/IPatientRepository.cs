using HospitalManagement.Models.Domain;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task<Patient?> CreateAsync(Patient patient);
        
        Task<Patient> GetByEmail(string email);
    }
}
