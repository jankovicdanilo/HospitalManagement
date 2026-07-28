using HospitalManagement.Shared.Models.Domain;

namespace HospitalManagement.QueryService.Repositories.Interfaces
{
    public interface IProcedureRepository
    {
        Task<(IEnumerable<Procedure> items, int totalCount)> GetAllAsync(int pageNumber, int pageSize);
        Task<Procedure?> GetByIdAsync(int id);
    }
}