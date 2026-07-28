using HospitalManagement.Shared.Models.DTOs.Procedure;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.QueryService.Services.Interfaces
{
    public interface IProcedureService
    {
        Task<Result<PagedResult<ProcedureListDto>>> GetAllAsync(int pageNumber, int pageSize);
        Task<Result<ProcedureResponseDto>> GetByIdAsync(int id);
    }
}