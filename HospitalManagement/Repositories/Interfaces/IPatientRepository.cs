using HospitalManagement.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task<Patient?> Delete(int id);

        Task<List<Patient>> GetAllAsync();

        Task<Patient?> GetByIdAsync(int id);

        Task<Patient?> CreateAsync(Patient patient);

        Task<Patient> UpdateAsync(Patient patient);

        bool PatientExists(int id);

        bool EmailExists(string email);

        Task<Patient?> GetByEmailAsync(string email);
    }
}
