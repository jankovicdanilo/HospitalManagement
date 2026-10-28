using HospitalManagement.Auth.Data;
using HospitalManagement.Auth.Models.Domain;
using HospitalManagement.Auth.Models.DTOs;
using HospitalManagement.Auth.Services.Interfaces;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Security;
using HospitalManagement.Shared.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HospitalManagement.Auth.Services.Implementations
{
    public class ServiceTokenService : IServiceTokenService
    {
        private const string TokenRole = "Mcp";

        private readonly AuthDbContext dbContext;
        private readonly JwtSettings jwtSettings;
        private readonly IDistributedCache distributedCache;
        private readonly ILogger<ServiceTokenService> logger;

        public ServiceTokenService(AuthDbContext dbContext, IOptions<JwtSettings> jwtSettings, 
            IDistributedCache distributedCache, ILogger<ServiceTokenService> logger)
        {
            this.dbContext = dbContext;
            this.jwtSettings = jwtSettings.Value;
            this.distributedCache = distributedCache;
            this.logger = logger;
        }

        public async Task<Result<ServiceTokenCreateResponseDto>> CreateAsync(ServiceTokenCreateRequestDto request, string createdBy)
        {
            var now = DateTime.UtcNow;
            var expires = now.AddDays(request.ExpiresInDays);
            var jti = Guid.NewGuid().ToString("N");
            var name = request.Name.Trim();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(ClaimTypes.NameIdentifier, $"service:{name}"),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, TokenRole)
            };

            var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(jwtSettings.Issuer, jwtSettings.Audience, claims, now, expires, credentials);

            var entity = new ServiceToken
            {
                Jti = jti,
                Name = name,
                Role = TokenRole,
                CreatedBy = createdBy,
                CreatedAtUtc = now,
                ExpiresAtUtc = expires
            };

            dbContext.ServiceTokens.Add(entity);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Service token {TokenId} ({Name}, role {Role}) created by {CreatedBy}, expires at {ExpiresAt}",
                entity.Id, name, TokenRole, createdBy, expires);

            var result = new ServiceTokenCreateResponseDto
            {
                Id = entity.Id,
                Name = name,
                Role = TokenRole,
                ExpiresAtUtc = expires,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };

            return Result<ServiceTokenCreateResponseDto>.Ok(result);
        }

        public async Task<Result<List<ServiceTokenListItemDto>>> GetAllAsync()
        {
            var now = DateTime.UtcNow;

            var items = await dbContext.ServiceTokens.AsNoTracking()
                .OrderByDescending(t => t.CreatedAtUtc).Select(t => new ServiceTokenListItemDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Role = t.Role,
                    CreatedBy = t.CreatedBy,
                    CreatedAtUtc = t.CreatedAtUtc,
                    ExpiresAtUtc = t.ExpiresAtUtc,
                    RevokedAtUtc = t.RevokedAtUtc,
                    Status = t.RevokedAtUtc != null ? "Revoked" : t.ExpiresAtUtc <= now ? "Expired" : "Active"
                }).ToListAsync();

            logger.LogInformation("Listed {Count} service tokens", items.Count);

            return Result<List<ServiceTokenListItemDto>>.Ok(items);
        }

        public async Task<Result> RevokeAsync(int id)
        {
            var entity = await dbContext.ServiceTokens.FirstOrDefaultAsync(t => t.Id == id);
            if(entity == null)
            {
                logger.LogWarning("Revoke requested for unknown service token {TokenId}", id);
                return Result.Fail($"Service token with id {id} not found", "INVALID_ID", ErrorType.NotFound);
            }

            if(entity.RevokedAtUtc != null || entity.ExpiresAtUtc <= DateTime.UtcNow)
            {
                logger.LogInformation("Service token {TokenId} is already revoked or expired", id);
                return Result.Ok("Token is already revoked or expired");
            }

            try
            {
                await distributedCache.SetStringAsync(TokenRevocation.KeyFor(entity.Jti), "1", new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.SpecifyKind(entity.ExpiresAtUtc, DateTimeKind.Utc))
                });
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Could not publish the revocation of service token {TokenId}", id);
                return Result.Fail("Could not revoke the token because the revocation list is unavailable",
                    "REVOCATION_FAILED", ErrorType.UpstreamFailure);
            }

            entity.RevokedAtUtc = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Service token {TokenId} ({Name}) revoked", entity.Id, entity.Name);

            return Result.Ok("Token revoked");
        }
    }
}
