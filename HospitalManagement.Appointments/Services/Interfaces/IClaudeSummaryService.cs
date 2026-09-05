namespace HospitalManagement.Appointments.Services.Interfaces
{
    public interface IClaudeSummaryService
    {
        Task<string> GenerateSummaryAsync(string prompt);
    }
}
