using HospitalManagement.Shared.Models.Domain;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface IProcedureRepository
    {
        Task<IEnumerable<Procedure>> GetAllAsync();
        Task<Procedure?> GetByIdAsync(int id);
        Task<Procedure> CreateAsync(Procedure request);
        Task<Procedure?> UpdateAsync(int id, Procedure request);
        Task<Procedure?> DeleteAsync(int id);
    }
}
