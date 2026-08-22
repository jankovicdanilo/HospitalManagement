using HospitalManagement.InvoiceService.Clients.Interfaces;
using HospitalManagement.InvoiceService.Models.DTOs.Invoice;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Models.Domain;
using System.Text.Json;

namespace HospitalManagement.InvoiceService.Clients.Implementations
{
    public class AppointmentServiceClient : IAppointmentServiceClient
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<AppointmentServiceClient> logger;

        public AppointmentServiceClient(HttpClient httpClient, ILogger<AppointmentServiceClient> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<AppointmentInvoiceDto?> GetAppointmentAsync(int appointmentId)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/appointment/{appointmentId}");
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("Failed to fetch appointment {Id}, status {Status}", appointmentId, response.StatusCode);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<AppointmentInvoiceDto>();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get appointment with id {Id}", appointmentId);
                return null;
            }

        }
    }
}
