using HospitalManagement.CommandService.Models.Domain;

namespace HospitalManagement.CommandService.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task<Patient?> CreateAsync(Patient patient);
        Task<Patient> UpdateAsync(Patient patient);
        Task<Patient?> Delete(int id);
        Task<bool> EmailExists(string email);
        Task<Patient?> GetByEmailAsync(string email);
        Task<bool> PatientExists(int id);
        Task<Patient?> GetByIdAsync(int id);
    }
}