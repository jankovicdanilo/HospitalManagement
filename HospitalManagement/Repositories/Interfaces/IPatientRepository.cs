using HospitalManagement.Models.Domain;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task<Patient> UpdateAsync(Patient patient);

        bool PatientExists(int id);

        bool EmailExists(string email);

        Task<Patient?> GetByEmailAsync(string email);

        Task<Patient?> GetByIdAsync(int id);
    }
}
