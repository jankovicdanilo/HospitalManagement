using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.Shared.Models.DTOs.Doctor;
using HospitalManagement.Shared.Models.DTOs.Patient;
using HospitalManagement.Statistics.Clients.Interfaces;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HospitalManagement.Statistics.Clients.Implementations
{
    public class QueryServiceClient : IQueryServiceClient
    {
        private const int BatchSize = 200; // Limit the number of IDs per request to avoid URL length issues

        private readonly HttpClient httpClient;
        private readonly ILogger<QueryServiceClient> logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public QueryServiceClient(HttpClient httpClient, ILogger<QueryServiceClient> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<DoctorResponseDto?> GetDoctorAsync(int doctorId)
        {
            var response = await httpClient.GetAsync($"api/doctor/{doctorId}");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogWarning("Doctor {DoctorId} not found at {Uri}", doctorId, response.RequestMessage?.RequestUri);
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DoctorResponseDto>(JsonOptions);
        }

        public async Task<PatientResponseDto?> GetPatientAsync(int patientId)
        {
            var response = await httpClient.GetAsync($"api/patient/{patientId}");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogWarning("Patient {PatientId} not found at {Uri}", patientId, response.RequestMessage?.RequestUri);
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PatientResponseDto>(JsonOptions);
        }

        public Task<IReadOnlyList<DoctorResponseDto>> GetDoctorsAsync(IEnumerable<int> doctorIds) => 
            PostBatchAsync<DoctorResponseDto>("api/doctor/batch", doctorIds);

        public Task<IReadOnlyList<PatientResponseDto>> GetPatientsAsync(IEnumerable<int> patientIds) =>
            PostBatchAsync<PatientResponseDto>("api/patient/batch", patientIds);

        private async Task<IReadOnlyList<T>> PostBatchAsync<T>(string path, IEnumerable<int> ids)
        {
            var requested = ids.Distinct().ToList();
            var result = new List<T>(requested.Count);

            foreach(var chunk in requested.Chunk(BatchSize))
            {
                var response = await httpClient.PostAsJsonAsync(path, new { ids = chunk });
                response.EnsureSuccessStatusCode();
                result.AddRange(await response.Content.ReadFromJsonAsync<List<T>>(JsonOptions) ?? []);
            }

            logger.LogInformation("Fetched {Count} of {Requested} records from {Path}", result.Count, requested.Count, path);
            return result;
        }


    }
}