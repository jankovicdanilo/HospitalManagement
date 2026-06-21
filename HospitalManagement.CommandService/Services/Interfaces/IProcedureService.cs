using HospitalManagement.CommandService.Models.Procedure;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.CommandService.Services.Interfaces
{
    public interface IProcedureService
    {
        Task<Result<ProcedureCreateResponseDto>> CreateAsync(ProcedureCreateRequestDto request);
        Task<Result<ProcedureUpdateResponseDto>> UpdateAsync(int id, ProcedureUpdateRequestDto request);
        Task<Result> DeleteAsync(int id);
    }
}