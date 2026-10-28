using HospitalManagement.Mcp.Api;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Globalization;

namespace HospitalManagement.Mcp.Tools
{
    [McpServerToolType]
    public class AppointmentTools(HospitalApi hospitalApi, ILogger<AppointmentTools> logger)
    {
        [McpServerTool(Name = "get_appointment", ReadOnly = true),
            Description("Gets one appointment by id, with doctor, patient, procedures and treatment.")]
        public Task<string> GetAppointment([Description("Appointment id")] int id)
        {
            logger.LogInformation("get_appointment {AppointmentId}", id);
            return hospitalApi.GetAsync(ApiNames.Appointments, $"api/appointment/{id}");
        }

        [McpServerTool(Name = "list_appointments", ReadOnly = true),
            Description("List appointments with optional filters. startDate and endDate must be given together; they take priority over date")]
        public Task<string> ListAppointments(
            [Description("Doctor id")] int? doctorId = null,
            [Description("Patient id")] int? patientId = null,
            [Description("A single day, yyyy-MM-dd")] string? date = null,
            [Description("Range start, yyyy-MM-dd")] string? startDate = null,
            [Description("Range end, yyyy-MM-dd")] string? endDate = null,
            [Description("Pending, Completed, Cancelled or Missed")] string? status = null,
            [Description("Page number, starting at 1")] int pageNumber = 1,
            [Description("Page size")] int pageSize = 20)
        {
            if((startDate == null) != (endDate == null))
            {
                logger.LogWarning("list_appointments called with only one of startDate/endDate");
                return Task.FromResult("Error: startDate and endDate must be provided together.");
            }

            logger.LogInformation("list_appointments page {Page} size {Size}", pageNumber, pageSize);
            return hospitalApi.GetAsync(ApiNames.Appointments, "api/appointment" + HospitalApi.BuildQuery(
                ("doctorId", doctorId), ("patientId", patientId), ("date", date), ("startDate", startDate), ("endDate", endDate),
                ("status", status), ("pageNumber", pageNumber), ("pageSize", pageSize)));
        }

        [McpServerTool(Name = "create_appointment"),
            Description("Creates an appointment. The doctor must work that weekday (check get_doctor_schedule or get_free_slots first) " +
            "and the slot must be free; the backend validates this. All times are UTC.")]
        public Task<string> CreateAppointment(
            [Description("Patient id")] int patientId,
            [Description("Doctor id")] int doctorId,
            [Description("Start time in UTC, ISO 8601 ending in Z, e.g. 2026-10-19T07:00:00Z.")] string startUtc,
            [Description("Duration in minutes, 1 to 480")] int durationMinutes,
            [Description("Optional notes, max 500 characters")] string? notes = null)
        {
            if(durationMinutes is < 1 or > 480)
            {
                logger.LogWarning("create_appointment rejected: duration {Minutes} out of range", durationMinutes);
                return Task.FromResult("Error: durationMinutes must be between 1 and 480.");
            }

            var input = startUtc.Trim();
            if (!input.EndsWith("Z", StringComparison.OrdinalIgnoreCase) ||
                !DateTime.TryParse(input, CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var start))
            {
                logger.LogWarning("create_appointment rejected: startUtc is not a UTC ISO 8601 value");
                return Task.FromResult("Error: startUtc must be UTC and end with Z, e.g. 2026-10-19T07:00:00Z.");
            }

            logger.LogInformation("create_appointment doctor {DoctorId} patient {PatientId} at {StartUtc} UTC for {Minutes} min",
                doctorId, patientId, startUtc, durationMinutes);

            return hospitalApi.PostAsync(ApiNames.Appointments, "api/appointment", new
            {
                patientId,
                doctorId,
                dateTime = start,
                duration = TimeSpan.FromMinutes(durationMinutes).ToString("c", CultureInfo.InvariantCulture),
                notes
            });
        }

        [McpServerTool(Name = "delete_appointment", Destructive = true), Description("Permanently deletes an appointment. " +
            "Only pass confirm=true after the user has explicitly asked to delete this appointment.")]
        public Task<string> DeleteAppointment(
            [Description("Appointment id")] int id,
            [Description("Must be true to actually delete")] bool confirm = false)
        {
            if (!confirm)
            {
                logger.LogWarning("delete_appointment {AppointmentId} refused: confirm was not set", id);
                return Task.FromResult($"Not deleted. Deleting appointment {id} cannot be undone; call again with confirm=true to proceed.");
            }

            logger.LogInformation("delete_appointment {AppointmentId} confirmed", id);
            return hospitalApi.DeleteAsync(ApiNames.Appointments, $"api/appointment/{id}");
        }
    }
}
