using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Patient;
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

        Task<bool> PatientExists(int id);

        Task<bool> EmailExists(string email);

        Task<Patient?> GetByEmailAsync(string email);

        Task<PatientMedicalHistoryDto> GetMedicalHistoryAsync(int patientId);
    }
}
