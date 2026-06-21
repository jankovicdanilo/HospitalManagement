using HospitalManagement.QueryService.Models.ReadModels;

namespace HospitalManagement.QueryService.Repositories.Interfaces
{
    public interface IProcedureRepository
    {
        Task<IEnumerable<ProcedureReadModel>> GetAllAsync();
        Task<ProcedureReadModel?> GetByIdAsync(int id);
    }
}