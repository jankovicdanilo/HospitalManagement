using HospitalManagement.Shared.Models.Domain;

namespace HospitalManagement.QueryService.Repositories.Interfaces
{
    public interface IProcedureRepository
    {
        Task<IEnumerable<Procedure>> GetAllAsync();
        Task<Procedure?> GetByIdAsync(int id);
    }
}