using HospitalManagement.Shared.Models.DTOs.Statistics;
using HospitalManagement.Statistics.Clients.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HospitalManagement.Statistics.Clients.Implementations
{
    public class AppointmentServiceClient : IAppointmentServiceClient
    {
        private readonly HttpClient httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public AppointmentServiceClient(HttpClient httpClient) => this.httpClient = httpClient;

        public async Task<List<AppointmentStatsRowDto>> GetStatsDataAsync(DateOnly from, DateOnly to)
        {
            var response = await httpClient.GetAsync(
                $"api/appointment/stats-data?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<AppointmentStatsRowDto>>(JsonOptions) ?? [];
        }
    }
}
