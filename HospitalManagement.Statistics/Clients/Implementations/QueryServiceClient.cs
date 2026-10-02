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
    }
}
