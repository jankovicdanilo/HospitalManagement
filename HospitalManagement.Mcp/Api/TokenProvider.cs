using HospitalManagement.Mcp.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Net.Http.Json;

namespace HospitalManagement.Mcp.Api
{
    public class TokenProvider(IHttpClientFactory httpClientFactory, IOptions<AuthOptions> authOptions,
        ILogger<TokenProvider> logger)
    {
        private readonly AuthOptions authOptions = authOptions.Value;
        private readonly SemaphoreSlim gate = new(1, 1);
        private string? token;

        public async Task<string> GetTokenAsync(bool forceRefresh = false)
        {
            if(token != null && !forceRefresh)
            {
                return token;
            }

            await gate.WaitAsync();
            try
            {
                if(token != null && !forceRefresh)
                {
                    return token;
                }

                logger.LogInformation("Logging in to the Auth service as {Username}", authOptions.Username);

                using var response = await httpClientFactory.CreateClient(ApiNames.Auth)
                    .PostAsJsonAsync("api/auth/login", new { username = authOptions.Username, password = authOptions.Password});
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError("Login failed with status {StatusCode}", (int)response.StatusCode);
                    throw new InvalidOperationException("Login response contained no token.");
                }

                var body = await response.Content.ReadFromJsonAsync<JsonElement>();
                token = body.GetProperty("token").GetString() ?? throw new InvalidOperationException("Login response contained no token.");

                logger.LogInformation("Login succeeded.");
                return token;
            }
            finally
            {
                gate.Release();
            }
        }
    }
}
