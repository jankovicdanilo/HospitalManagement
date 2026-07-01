using HospitalManagement.Shared.Models.DTOs.Procedure;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.QueryService.Services.Interfaces
{
    public interface IProcedureService
    {
        Task<Result<List<ProcedureListDto>>> GetAllAsync();
        Task<Result<ProcedureResponseDto>> GetByIdAsync(int id);
    }
}