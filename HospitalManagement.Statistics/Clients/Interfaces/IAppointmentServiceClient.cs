using HospitalManagement.Shared.Models.DTOs.Statistics;

namespace HospitalManagement.Statistics.Clients.Interfaces
{
    public interface IAppointmentServiceClient
    {
        Task<List<AppointmentStatsRowDto>> GetStatsDataAsync(DateOnly from, DateOnly to);
    }
}
