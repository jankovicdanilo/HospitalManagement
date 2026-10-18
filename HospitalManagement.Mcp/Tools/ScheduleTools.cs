using HospitalManagement.Mcp.Api;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace HospitalManagement.Mcp.Tools
{
    [McpServerToolType]
    public class ScheduleTools(HospitalApi hospitalApi, ILogger<ScheduleTools> logger)
    {
        [McpServerTool(Name = "get_doctor_schedule", ReadOnly = true),
            Description("Gets a doctors weekly schedule: the weekdays they work and their working hours in clinic local time. " +
            "Check this before booking an appointment. An empty result means the doctor works on no day yet.")]
        public Task<string> GetDoctorSchedule([Description("Doctor id")] int doctorId)
        {
            logger.LogInformation("get_doctor_schedule {DoctorId}", doctorId);
            return hospitalApi.GetAsync(ApiNames.Query, $"api/doctorschedule/doctor/{doctorId}");
        }

        [McpServerTool(Name = "create_doctor_schedule"),
            Description("Sets working hours for a doctor on one or more weekdays (Monday to Friday only; weekends are not " +
            "allowed). startHour must be 8 to 19 and endHour 9 to 20, after startHour. A newly created doctor has no schedule, " +
            "and no appointment can be booked until this is done. Hours are clinic local time.")]
        public async Task<string> CreateDoctorSchedule(
            [Description("Doctor id")] int doctorId,
            [Description("Comma-separated weekday names, Monday to Friday, e.g. Monday,Tuesday,Wednesday")] string days,
            [Description("First working hour, 8 to 19 (e.g. 9)")] int startHour,
            [Description("Hour work ends, 9 to 20, after startHour (e.g. 17)")] int endHour)
        {
            if(doctorId <= 0)
            {
                logger.LogWarning("create_doctor_schedule rejected: doctorId {DoctorId} must be greater than 0", doctorId);
                return "Error: doctorId must be greater than 0.";
            }

            if(startHour is < 8 or > 19 || endHour is < 9 or > 20 || endHour <= startHour)
            {
                logger.LogWarning("create_doctor_schedule rejected: hours {Start}-{End} are invalid", startHour, endHour);
                return "Error: startHour must be 8-19, endHour 9-20, and endHour must be after startHour.";
            }

            var weekdays = new List<DayOfWeek>();
            foreach(var part in days.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if(!part.All(char.IsLetter) || !Enum.TryParse<DayOfWeek>(part, ignoreCase: true, out var day))
                {
                    logger.LogWarning("create_doctor_schedule rejected: '{Day}' is not a weekday", part);
                    return $"Error: '{part}' is not a weekday name. Use english names such as Monday";
                }

                if(day is DayOfWeek.Saturday or DayOfWeek.Sunday)
                {
                    logger.LogWarning("create_doctor_schedule rejected: {Day} is a weekend day", day);
                    return "Error: schedules can only be set for Monday to Friday";
                }

                if (!weekdays.Contains(day))
                {
                    weekdays.Add(day);
                }
            }

            if(weekdays.Count == 0)
            {
                return "Error: provide at least one weekday, Monday to Friday.";
            }

            logger.LogInformation("create_doctor_schedule doctor {DoctorId}: {Days} from {Start} to {End}",
                doctorId, string.Join(",", weekdays), startHour, endHour);

            var lines = new List<string>();
            foreach(var day in weekdays)
            {
                var result = await hospitalApi.PostAsync(ApiNames.Command, "api/doctorschedule",
                    new { doctorId, dayOfWeek = day.ToString(), startHour, endHour });

                var failed = result.StartsWith("Error", StringComparison.Ordinal);
                logger.LogInformation("Schedule for doctor {DoctorId} on {Day}: {Outcome}",
                    doctorId, day, failed ? "failed" : "created");

                lines.Add(failed ? $"{day}: {result}" : $"{day}: created");
            }

            return string.Join("\n", lines);
        }

        [McpServerTool(Name = "get_free_slots", ReadOnly = true),
            Description("Lists a doctor's free appointment slots on one date. Fails with a readable error if the doctor " +
            "does not work that weekday or the date is in the past.")]
        public Task<string> GetFreeSlots(
            [Description("Doctor id")] int doctorId,
            [Description("Date, yyyy-MM-dd")] string date)
        {
            if(!DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                logger.LogWarning("get_free_slots rejected: date could not be parsed");
                return Task.FromResult("Error: date must be in the format yyyy-MM-dd.");
            }

            logger.LogInformation("get_free_slots doctor {DoctorId} date {Date}", doctorId, date);

            return hospitalApi.GetAsync(ApiNames.Appointments, "api/appointment/free-slots" +
                HospitalApi.BuildQuery(("doctorId", doctorId), ("date", date)));
        }
    }
}
