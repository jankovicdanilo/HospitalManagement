using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HospitalManagement.Mcp.Api
{
    public class TokenProvider(IHttpClientFactory httpClientFactory, IConfiguration configuration,
        ILogger<TokenProvider> logger)
    {
        private readonly SemaphoreSlim gate = new(1, 1);
        private string? token;

        public async Task<string> GetTokenAsync(bool forceRefresh = false)
        {

        }
    }
}
