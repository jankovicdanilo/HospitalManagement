using HospitalManagement.Auth.Models.DTOs;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.Auth.Services.Interfaces
{
    public interface IServiceTokenService
    {
        Task<Result<ServiceTokenCreateResponseDto>> CreateAsync(ServiceTokenCreateRequestDto request, string createdBy);
        Task<Result<List<ServiceTokenListItemDto>>> GetAllAsync();
        Task<Result> RevokeAsync(int id);
    }
}
