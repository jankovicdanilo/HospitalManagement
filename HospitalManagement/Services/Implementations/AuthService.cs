using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Auth;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;
using HospitalManagement.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HospitalManagement.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository authRepository;
        private readonly JwtSettings jwtSettings;

        public AuthService(IAuthRepository authRepository, IOptions<JwtSettings> jwtSettings)
        {
            this.authRepository = authRepository;
            this.jwtSettings = jwtSettings.Value;
        }

        public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
        {
            var existingUser = await authRepository.GetByUsernameAsync(request.Username);

            if(existingUser != null)
            {
                return Result<AuthResponseDto>.Fail
                    ($"Username {request.Username} already exists", "USERNAME_TAKEN");
            }

            var existingEmail = await authRepository.GetByEmailAsync(request.Email);

            if(existingEmail != null)
            {
                return Result<AuthResponseDto>.Fail($"Email {request.Email} already exists", "EMAIL_TAKEN");
            }

            if(string.IsNullOrEmpty(request.Username))
            {
                return Result<AuthResponseDto>.Fail($"Username can't be null", "USERNAME_NULL");
            }

            if (string.IsNullOrEmpty(request.Email))
            {
                return Result<AuthResponseDto>.Fail($"Email can't be null", "EMAIL_NULL");
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                return Result<AuthResponseDto>.Fail($"Password can't be null", "PASSWORD_NULL");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = request.Role
            };

            await authRepository.CreateAsync(user);

            var token = GenerateToken(user);

            var result = new AuthResponseDto
                (
                    user.Username,
                    user.Email,
                    user.Role,
                    token
                );

            return Result<AuthResponseDto>.Ok(result);
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request)
        {
            var user = await authRepository.GetByUsernameAsync(request.Username);

            if(user == null)
            {
                return Result<AuthResponseDto>.Fail("Invalid credentials", "INVALID_CREDENTIALS");
            }

            var isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password,user.PasswordHash);

            if (!isValidPassword)
            {
                return Result<AuthResponseDto>.Fail("Invalid credentials", "INVALID_CREDENTIALS");
            }

            var token = GenerateToken(user);

            var result = new AuthResponseDto
            (
                user.Username,
                user.Email,
                user.Role,
                token
            );

            return Result<AuthResponseDto>.Ok(result);
        }

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryMinutes),
            signingCredentials: credentials
            );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Result<List<AuthResponseListDto>>> GetAllAsync()
        {
            var users = await authRepository.GetAllAsync();

            var result = users.Select(user => new AuthResponseListDto
            (
                user.Id,
                user.Username,
                user.Email,
                user.Role,
                user.CreatedAt,
                user.IsActive
            )).ToList();

            return Result<List<AuthResponseListDto>>.Ok(result);
        }

        public async Task<Result<CurrentUserDto>> GetCurrentUserAsync(int id)
        {
            var user = await authRepository.GetByIdAsync(id);

            if(user == null)
            {
                return Result<CurrentUserDto>.Fail($"User with the id {id} not found", "USER_NOT_FOUND");
            }

            var result = new CurrentUserDto
                (
                    user.Id,
                    user.Username,
                    user.Email,
                    user.Role,
                    user.CreatedAt
                );

            return Result<CurrentUserDto>.Ok(result);
        }

        public async Task<Result> DeleteUser(int id)
        {
            if(await authRepository.GetByIdAsync(id) == null)
            {
                return Result.Fail($"User with the {id} not found", "USER_NOT_FOUND");
            }

            await authRepository.Delete(id);

            return Result.Ok("User deleted");
        }

        public async Task<Result<AuthResponseUpdateDto>> UpdateUserAsync(UpdateUserRequestDto request)
        {
            var user = await authRepository.GetByIdAsync(request.Id);

            if (user == null)
            {
                return Result<AuthResponseUpdateDto>.Fail
                    ($"User with the {request.Id} not found", "USER_NOT_FOUND");
            }

            user.Username = request.Username;
            user.Email = request.Email;
            user.Role = request.Role;
            user.IsActive = request.IsActive;

            var updatedUser = await authRepository.UpdateAsync(user);

            var result = new AuthResponseUpdateDto
            (
                updatedUser.Id,
                updatedUser.Username,
                updatedUser.Email,
                updatedUser.Role,
                updatedUser.IsActive,
                updatedUser.CreatedAt
            );

            return Result< AuthResponseUpdateDto>.Ok(result);
        }
    }
}
