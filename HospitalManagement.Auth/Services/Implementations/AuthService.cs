using AutoMapper;
using HospitalManagement.Auth.Models.Domain;
using HospitalManagement.Auth.Models.DTOs;
using HospitalManagement.Auth.Repositories.Interfaces;
using HospitalManagement.Auth.Services.Interfaces;
using HospitalManagement.Shared.Common;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HospitalManagement.Shared.Settings;

namespace HospitalManagement.Auth.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository authRepository;
        private readonly JwtSettings jwtSettings;
        private readonly IMapper mapper;
        private readonly ILogger<AuthService> logger;

        public AuthService(IAuthRepository authRepository, IOptions<JwtSettings> jwtSettings, IMapper mapper,
            ILogger<AuthService> logger)
        {
            this.authRepository = authRepository;
            this.jwtSettings = jwtSettings.Value;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
        {
            var existingUser = await authRepository.GetByUsernameAsync(request.Username);

            if (existingUser != null)
            {
                logger.LogWarning("Registration failed, username {Username} already exists", request.Username);
                return Result<AuthResponseDto>.Fail
                    ($"Username {request.Username} already exists", "USERNAME_TAKEN");
            }

            var existingEmail = await authRepository.GetByEmailAsync(request.Email);

            if (existingEmail != null)
            {
                logger.LogWarning("Registration failed, email {Email} already exists", request.Email);
                return Result<AuthResponseDto>.Fail($"Email {request.Email} already exists", "EMAIL_TAKEN");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = mapper.Map<User>(request);
            user.PasswordHash = passwordHash;
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;

            await authRepository.CreateAsync(user);

            logger.LogInformation("User registered with username {Username}", user.Username);

            var token = GenerateToken(user);

            var result = mapper.Map<AuthResponseDto>(user);
            result.Token = token;

            return Result<AuthResponseDto>.Ok(result);
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request)
        {
            var user = await authRepository.GetByUsernameAsync(request.Username);

            if (user == null)
            {
                logger.LogWarning("Login failed, username {Username} not found", request.Username);
                return Result<AuthResponseDto>.Fail("Invalid credentials", "INVALID_CREDENTIALS");
            }

            var isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isValidPassword)
            {
                logger.LogWarning("Login failed, invalid password for username {Username}", request.Username);
                return Result<AuthResponseDto>.Fail("Invalid credentials", "INVALID_CREDENTIALS");
            }

            logger.LogInformation("User {Username} logged in", user.Username);

            var token = GenerateToken(user);

            var result = mapper.Map<AuthResponseDto>(user);
            result.Token = token;

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

            var result = mapper.Map<List<AuthResponseListDto>>(users);

            return Result<List<AuthResponseListDto>>.Ok(result);
        }

        public async Task<Result<CurrentUserDto>> GetCurrentUserAsync(int id)
        {
            var user = await authRepository.GetByIdAsync(id);

            if (user == null)
            {
                logger.LogWarning("User with id {Id} not found", id);
                return Result<CurrentUserDto>.Fail($"User with the id {id} not found", "USER_NOT_FOUND");
            }

            var result = mapper.Map<CurrentUserDto>(user);

            return Result<CurrentUserDto>.Ok(result);
        }

        public async Task<Result> DeleteUser(int id)
        {
            if (await authRepository.GetByIdAsync(id) == null)
            {
                logger.LogWarning("User with id {Id} not found for deletion", id);
                return Result.Fail($"User with the {id} not found", "USER_NOT_FOUND");
            }

            await authRepository.Delete(id);

            logger.LogInformation("User with id {Id} deleted", id);

            return Result.Ok("User deleted");
        }

        public async Task<Result<AuthResponseUpdateDto>> UpdateUserAsync(UpdateUserRequestDto request)
        {
            var user = await authRepository.GetByIdAsync(request.Id);

            if (user == null)
            {
                logger.LogWarning("User with id {Id} not found for update", request.Id);
                return Result<AuthResponseUpdateDto>.Fail
                    ($"User with the {request.Id} not found", "USER_NOT_FOUND");
            }

            mapper.Map(request, user);

            user = await authRepository.UpdateAsync(user);

            logger.LogInformation("User with id {Id} updated", user.Id);

            var result = mapper.Map<AuthResponseUpdateDto>(user);

            return Result<AuthResponseUpdateDto>.Ok(result);
        }
    }
}
