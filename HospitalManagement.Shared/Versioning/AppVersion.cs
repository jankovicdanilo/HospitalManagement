using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Shared.Versioning
{
    public static class AppVersion
    {
        public static string Version { get; } = Environment.GetEnvironmentVariable("APP_VERSION") ?? "dev";

        public static string Build { get; } = Environment.GetEnvironmentVariable("APP_BUILD") ?? "local";

        public static IEndpointRouteBuilder MapVersionEndpoint(this IEndpointRouteBuilder endpoints, string serviceName)
        {
            var logger = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("HospitalManagement.Version");
            logger.LogInformation("{Service} version {Version} (build {Build})", serviceName, Version, Build);

            endpoints.MapGet("/version", () => Results.Ok(new { service = serviceName, version = Version, build = Build }))
                .AllowAnonymous();

            return endpoints;
        }
    }
}
