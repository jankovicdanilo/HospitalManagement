using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace HospitalManagement.Mcp.Api
{
    public class HospitalApi(IHttpClientFactory httpClientFactory, TokenProvider tokenProvider,
        ILogger<HospitalApi> logger)
    {
        public Task<string> GetAsync(string service, string path) =>
            SendAsync(service, HttpMethod.Get, path, null);
        public Task<string> PostAsync(string service, string path, object body) =>
            SendAsync(service, HttpMethod.Post, path, body);
        public Task<string> DeleteAsync(string service, string path) =>
            SendAsync(service, HttpMethod.Delete, path, null);

        public static string BuildQuery(params(string Key, object? Value)[] pairs)
        {
            var parts = pairs
                .Where(p => p.Value != null)
                .Select(p => $"{Uri.EscapeDataString(p.Key)}=" +
                             $"{Uri.EscapeDataString(
                                 Convert.ToString(p.Value, CultureInfo.InvariantCulture)!)}");

            var query = string.Join("&", parts);
            return query.Length == 0 ? string.Empty : "?" + query;
        }

        private async Task<string> SendAsync(string service, HttpMethod httpMethod,
            string path, object? body)
        {
            // query strings can contain names, so keep them out of the logs
            var pathOnly = path.Split('?')[0]; 

            for(var attempt = 0; attempt < 2; attempt++)
            {
                var token = await tokenProvider.GetTokenAsync(forceRefresh: attempt > 0);

                using var request = new HttpRequestMessage(httpMethod, path);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                if(body != null)
                {
                    request.Content = JsonContent.Create(body);
                }

                logger.LogInformation("{Method} {Service}/{Path}", httpMethod, service, pathOnly);

                using var response = await httpClientFactory.CreateClient(service).SendAsync(request);
                var text = await response.Content.ReadAsStringAsync();

                if(response.StatusCode == HttpStatusCode.Unauthorized && attempt == 0)
                {
                    logger.LogWarning("401 from {Service}, refreshing the token " +
                        "and retrying once", service);
                    continue;
                }

                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("{Method} {Service}/{Path} -> {StatusCode}",
                        httpMethod, service, pathOnly, (int)response.StatusCode);
                    return string.IsNullOrWhiteSpace(text) ? "OK" : text;
                }

                logger.LogWarning("{Method} {Service}/{Path} failed with {StatusCode}",
                    httpMethod, service, pathOnly, (int)response.StatusCode);

                return FormatError(response.StatusCode, text);
            }

            return "Error 401: not authorized.";
        }

        private static string FormatError(HttpStatusCode statusCode, string body)
        {
            var code = (int)statusCode;
            if (string.IsNullOrWhiteSpace(body))
            {
                return $"Error {code}: the request failed with no details.";
            }

            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;
                var message = root.TryGetProperty("message", out var m) ? m.GetString()
                    : root.TryGetProperty("title", out var t) ? t.GetString() 
                    : null;
                var errorCode = root.TryGetProperty("errorCode", out var c) ? c.GetString() : null;

                var sb = new StringBuilder($"Error {code} {errorCode}: {message}");
                if(root.TryGetProperty("errors", out var errors) && 
                    errors.ValueKind == JsonValueKind.Object)
                {
                    foreach(var field in errors.EnumerateObject())
                    {
                        sb.Append($"\n- {field.Name}: {string.Join("; ",
                            field.Value.EnumerateArray().Select(e => e.GetString()))}");
                    }
                }

                return sb.ToString();
            }
            catch(Exception ex) when (ex is JsonException or InvalidOperationException)
            {
                return $"Error {code}: {body}";
            }
        }
    }
}
