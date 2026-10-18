using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace HospitalManagement.Mcp.Api
{
    public class HospitalApi(IHttpClientFactory httpClientFactory, ILogger<HospitalApi> logger)
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
            var pathOnly = path.Split('?')[0]; 

            using var request = new HttpRequestMessage(httpMethod, path);
            if (body != null)
            {
                request.Content = JsonContent.Create(body);
            }

            try
            {
                using var response = await httpClientFactory.CreateClient(service).SendAsync(request);
                var text = await response.Content.ReadAsStringAsync();

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
            catch (Exception ex) when (ex is HttpRequestException or TimeoutRejectedException or BrokenCircuitException)
            {
                logger.LogError(ex, "{Method} {Service}/{Path} could not be completed", httpMethod, service, pathOnly);
                return $"Error: {service} could not be reached. {ex.Message}";
            }
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
