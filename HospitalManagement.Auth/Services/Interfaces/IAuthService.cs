using HospitalManagement.Auth.Models.DTOs;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.Auth.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);

        Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request);

        Task<Result<List<AuthResponseListDto>>> GetAllAsync();

        Task<Result<CurrentUserDto>> GetCurrentUserAsync(int userId);

        Task<Result> DeleteUser(int id);

        Task<Result<AuthResponseUpdateDto>> UpdateUserAsync(UpdateUserRequestDto request);
    }
}
