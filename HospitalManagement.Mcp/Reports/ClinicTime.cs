using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagement.Mcp.Reports
{
    public static class ClinicTime
    {
        public static readonly TimeZoneInfo TimeZone = Resolve();

        public static bool IsValid(DateTime local) =>
            TimeZone.IsInvalidTime(DateTime.SpecifyKind(local, DateTimeKind.Unspecified));

        public static DateTime ToUtc(DateTime local) =>
            TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(local, DateTimeKind.Unspecified), TimeZone);

        private static TimeZoneInfo Resolve()
        {
            foreach(var id in new[] {"Europe/Podgorica", "Central Europe Standard Time" })
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(id);
                }
                catch (TimeZoneNotFoundException) { }
                catch (InvalidTimeZoneException) { }

            }

            throw new InvalidOperationException("The clinic time zone (Europe/Podgorica) was not found on this machine.");
        }
    }
}
