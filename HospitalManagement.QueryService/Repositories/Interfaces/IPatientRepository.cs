using HospitalManagement.QueryService.Models.ReadModels;

namespace HospitalManagement.QueryService.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task<List<PatientReadModel>> GetAllAsync();
        Task<PatientReadModel?> GetByIdAsync(int id);
        Task<PatientReadModel?> GetByEmailAsync(string email);
    }
}