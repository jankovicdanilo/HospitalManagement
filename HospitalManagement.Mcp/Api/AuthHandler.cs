using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.Extensions.Options;
using HospitalManagement.Mcp.Options;

namespace HospitalManagement.Mcp.Api
{
    public class AuthHandler(IOptions<AuthOptions> authOptions, ILogger<AuthHandler> logger) : DelegatingHandler
    {
         protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authOptions.Value.Token);

            var response = await base.SendAsync(request, cancellationToken);
            if(response.StatusCode == HttpStatusCode.Unauthorized)
            {
                logger.LogWarning("{Path} returned 401: the configured Auth:Token is expired, revoked or invalid", request.RequestUri?.AbsolutePath);
            }

            return response;
        }
    }
}
