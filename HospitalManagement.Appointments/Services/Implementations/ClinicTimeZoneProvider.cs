using HospitalManagement.Appointments.Services.Interfaces;
using HospitalManagement.Appointments.Settings;
using Microsoft.Extensions.Options;

namespace HospitalManagement.Appointments.Services.Implementations
{
    public class ClinicTimeZoneProvider : IClinicTimeZoneProvider
    {
        private readonly TimeZoneInfo timeZone;

        public ClinicTimeZoneProvider(IOptions<ClinicSettings> settings)
        {
            timeZone = TimeZoneInfo.FindSystemTimeZoneById(settings.Value.TimeZoneId);
        }

        public DateTime ToLocal(DateTime utc) =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), timeZone);

        public DateTime ToUtc(DateTime local) =>
            TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(local, DateTimeKind.Unspecified), timeZone);

    }
}
