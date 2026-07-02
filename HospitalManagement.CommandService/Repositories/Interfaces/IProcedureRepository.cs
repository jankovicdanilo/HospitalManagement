using HospitalManagement.Shared.Models.Domain;

namespace HospitalManagement.CommandService.Repositories.Interfaces
{
    public interface IProcedureRepository
    {
        Task<Procedure> CreateAsync(Procedure request);
        Task<Procedure?> UpdateAsync(int id, Procedure request);
        Task<Procedure?> DeleteAsync(int id);
    }
}