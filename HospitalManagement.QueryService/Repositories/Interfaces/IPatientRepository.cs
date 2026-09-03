using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.Shared.Models.DTOs.Patient;

namespace HospitalManagement.QueryService.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task<(List<Patient> items, int totalCount)> GetAllAsync(PatientFilterDto filter);
        Task<Patient?> GetByIdAsync(int id);
        Task<Patient?> GetByEmailAsync(string email);
        Task<List<Patient>> GetByIdsAsync(List<int> ids);
    }
}