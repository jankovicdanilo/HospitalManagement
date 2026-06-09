
using HospitalManagement.Models.DTOs.Procedure;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.Services.Interfaces
{
    public interface IProcedureService
    {
        Task<Result<List<ProcedureListDto>>> GetAllAsync();
        Task<Result<ProcedureResponseDto>> GetByIdAsync(int id);
        Task<Result> DeleteAsync(int id);
        Task<Result<ProcedureCreateResponseDto>> CreateAsync(ProcedureCreateRequestDto request);
        Task<Result<ProcedureUpdateResponseDto>> UpdateAsync(int id, ProcedureUpdateRequestDto request);
    }
}
