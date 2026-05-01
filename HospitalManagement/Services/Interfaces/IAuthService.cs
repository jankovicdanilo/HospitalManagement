using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Auth;

namespace HospitalManagement.Services.Interfaces
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
