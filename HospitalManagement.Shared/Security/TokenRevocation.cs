

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HospitalManagement.Shared.Security
{
    public static class TokenRevocation
    {
        public const string KeyPrefix = "revoked-token:";
        public static string KeyFor(string jti) => KeyPrefix + jti;

        public static async Task CheckAsync(TokenValidatedContext context)
        {
            var jti = context.Principal?.FindFirst("jti")?.Value;
            if (string.IsNullOrEmpty(jti))
            {
                return;
            }

            var services = context.HttpContext.RequestServices;
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("TokenRevocation");
            var distributedCache = services.GetService<IDistributedCache>();

            if(distributedCache == null)
            {
                logger.LogWarning("Service token {Jti} rejected: this service cannot check the revocation list", jti);
                context.Fail("Service tokens are not accepted by this service");
                return;
            }

            try
            {
                if(await distributedCache.GetStringAsync(KeyFor(jti)) != null)
                {
                    logger.LogWarning("Revoked service token {Jti} rejected", jti);
                    context.Fail("Token has been revoked");
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Could not check the revocation list for token {Jti}; rejecting it", jti);
                context.Fail("Token revocation status could not be verified.");
            }
        }
    }
}
