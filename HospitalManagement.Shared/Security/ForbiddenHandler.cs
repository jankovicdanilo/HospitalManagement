using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace HospitalManagement.Shared.Security
{
    public static class ForbiddenHandler
    {
        public static async Task HandleAsync(ForbiddenContext context)
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILoggerFactory>().CreateLogger("Authorization");
            var user = context.HttpContext.User;

            logger.LogWarning("Forbidden: {User} (role {Role}) tried {Method} {Path}",
                user.Identity?.Name ?? "unknown",
                user.FindFirst(ClaimTypes.Role)?.Value ?? "none",
                context.Request.Method,
                context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                ErrorCode = "FORBIDDEN",
                Message = "You do not have permission to perform this action"
            });
        }
    }
}
