using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Models.DTOs.Statistics;
using HospitalManagement.Shared.Models.Enums;
using HospitalManagement.Statistics.Clients.Interfaces;
using System.Globalization;

namespace HospitalManagement.Statistics.Services.Utility
{
    public class StatisticsBuilder
    {
        private readonly IAppointmentServiceClient appointmentClient;
        private readonly IQueryServiceClient queryClient;
        private readonly ILogger<StatisticsBuilder> logger;

        public StatisticsBuilder(IAppointmentServiceClient appointmentClient,
            IQueryServiceClient queryClient, ILogger<StatisticsBuilder> logger)
        {
            this.appointmentClient = appointmentClient;
            this.queryClient = queryClient;
            this.logger = logger;
        }

        public async Task<Result<List<DoctorLoadTimelineDto>>> BuildDoctorsLoadTimelineAsync(
    DateOnly from, DateOnly to, int top)
        {
            if (from > to)
            {
                return Result<List<DoctorLoadTimelineDto>>.Fail("From must not be after to",
                    "INVALID_DATE_RANGE", ErrorType.Validation);
            }

            if (top < 1 || top > 50)
            {
                return Result<List<DoctorLoadTimelineDto>>.Fail("Top must be between 1 and 50",
                    "INVALID_TOP", ErrorType.Validation);
            }

            try
            {
                var rows = await appointmentClient.GetStatsDataAsync(from, to);
                var visits = rows.Where(r => r.Status != AppointmentStatus.Cancelled).ToList();

                var topDoctors = visits
                    .GroupBy(r => r.DoctorId)
                    .Select(g => new { DoctorId = g.Key, Total = g.Count() })
                    .OrderByDescending(x => x.Total)
                    .ThenBy(x => x.DoctorId)
                    .Take(top)
                    .ToList();

                if (topDoctors.Count == 0)
                {
                    return Result<List<DoctorLoadTimelineDto>>.Ok([]);
                }

                var topIds = topDoctors.Select(d => d.DoctorId).ToHashSet();
                var names = await GetDoctorNamesAsync(topIds);

                var selected = visits.Where(r => topIds.Contains(r.DoctorId)).ToList();
                var periods = BuildMonthAxis(selected.Select(r => r.DateTime));

                var result = topDoctors.Select(d =>
                {
                    var counts = selected
                        .Where(r => r.DoctorId == d.DoctorId)
                        .GroupBy(r => MonthKey(r.DateTime))
                        .ToDictionary(g => g.Key, g => g.Count());

                    return new DoctorLoadTimelineDto
                    {
                        DoctorId = d.DoctorId,
                        DoctorName = names[d.DoctorId],
                        TotalAppointments = d.Total,
                        Points = periods
                            .Select(p => new VisitsOverTimeDto { Period = p, Visits = counts.GetValueOrDefault(p) })
                            .ToList()
                    };
                }).ToList();

                return Result<List<DoctorLoadTimelineDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to build doctors load timeline");
                return Result<List<DoctorLoadTimelineDto>>.Fail("Failed to load statistics data",
                    "UPSTREAM_FAILURE", ErrorType.UpstreamFailure);
            }
        }

        public async Task<Result<DoctorStatisticsDto>> BuildDoctorStatisticsAsync(
    int doctorId, DateOnly from, DateOnly to)
        {
            if (from > to)
            {
                return Result<DoctorStatisticsDto>.Fail("From must not be after to",
                    "INVALID_DATE_RANGE", ErrorType.Validation);
            }

            try
            {
                var rows = (await appointmentClient.GetStatsDataAsync(from, to))
                    .Where(r => r.DoctorId == doctorId)
                    .ToList();

                var doctor = await queryClient.GetDoctorAsync(doctorId);

                if (doctor == null && rows.Count == 0)
                {
                    return Result<DoctorStatisticsDto>.Fail($"Doctor with id {doctorId} not found",
                        "INVALID_DOCTOR_ID", ErrorType.NotFound);
                }

                var visits = rows.Where(r => r.Status != AppointmentStatus.Cancelled).ToList();
                var completed = rows.Where(r => r.Status == AppointmentStatus.Completed).ToList();

                var topPatients = visits
                    .GroupBy(r => r.PatientId)
                    .Select(g => new
                    {
                        PatientId = g.Key,
                        Visits = g.Count(),
                        Revenue = g.Where(r => r.Status == AppointmentStatus.Completed).Sum(r => r.TotalCost)
                    })
                    .OrderByDescending(x => x.Revenue)
                    .ThenByDescending(x => x.Visits)
                    .Take(5)
                    .ToList();

                var patientNames = await GetPatientNamesAsync(topPatients.Select(p => p.PatientId));

                var periods = BuildMonthAxis(visits.Select(r => r.DateTime));
                var monthCounts = visits
                    .GroupBy(r => MonthKey(r.DateTime))
                    .ToDictionary(g => g.Key, g => g.Count());

                var result = new DoctorStatisticsDto
                {
                    DoctorId = doctorId,
                    DoctorName = doctor != null
                        ? $"{doctor.FirstName} {doctor.LastName}"
                        : $"Unknown doctor ({doctorId})",
                    AppointmentCount = visits.Count,
                    PendingCount = rows.Count(r => r.Status == AppointmentStatus.Pending),
                    CompletedCount = completed.Count,
                    MissedCount = rows.Count(r => r.Status == AppointmentStatus.Missed),
                    CancelledCount = rows.Count(r => r.Status == AppointmentStatus.Cancelled),
                    Revenue = completed.Sum(r => r.TotalCost),
                    UniquePatients = visits.Select(r => r.PatientId).Distinct().Count(),
                    VisitsOverTime = periods
                        .Select(p => new VisitsOverTimeDto { Period = p, Visits = monthCounts.GetValueOrDefault(p) })
                        .ToList(),
                    TopProcedures = completed
                        .SelectMany(r => r.Procedures)
                        .GroupBy(p => p.ProcedureId)
                        .Select(g => new ProcedureProfitabilityDto
                        {
                            ProcedureId = g.Key,
                            ProcedureName = g.First().ProcedureName,
                            TimesPerformed = g.Count(),
                            Revenue = g.Sum(p => p.ProcedurePrice)
                        })
                        .OrderByDescending(x => x.Revenue)
                        .Take(5)
                        .ToList(),
                    TopPatients = topPatients.Select(p => new TopPatientDto
                    {
                        PatientId = p.PatientId,
                        PatientName = patientNames[p.PatientId],
                        Visits = p.Visits,
                        Revenue = p.Revenue
                    }).ToList()
                };

                return Result<DoctorStatisticsDto>.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to build statistics for doctor {DoctorId}", doctorId);
                return Result<DoctorStatisticsDto>.Fail("Failed to load statistics data",
                    "UPSTREAM_FAILURE", ErrorType.UpstreamFailure);
            }
        }

        public async Task<Result<List<DoctorLoadDto>>> BuildDoctorsLoadAsync(DateOnly from, DateOnly to)
        {
            if (from > to)
            {
                return Result<List<DoctorLoadDto>>.Fail("From must not be after to",
                    "INVALID_DATE_RANGE", ErrorType.Validation);
            }

            try
            {
                var rows = await appointmentClient.GetStatsDataAsync(from, to);

                var counts = rows
                    .Where(r => r.Status != AppointmentStatus.Cancelled)
                    .GroupBy(r => r.DoctorId)
                    .ToDictionary(g => g.Key, g => g.Count());

                var names = await GetDoctorNamesAsync(counts.Keys);

                var result = counts
                    .Select(kv => new DoctorLoadDto
                    {
                        DoctorId = kv.Key,
                        DoctorName = names[kv.Key],
                        AppointmentCount = kv.Value
                    })
                    .OrderByDescending(x => x.AppointmentCount)
                    .ToList();

                return Result<List<DoctorLoadDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to build doctors load statistics");
                return Result<List<DoctorLoadDto>>.Fail("Failed to load statistics data",
                    "UPSTREAM_FAILURE", ErrorType.UpstreamFailure);
            }
        }

        public async Task<Result<List<DoctorRevenueDto>>> BuildDoctorsRevenueAsync(DateOnly from, DateOnly to)
        {
            if (from > to)
            {
                return Result<List<DoctorRevenueDto>>.Fail("From must not be after to",
                    "INVALID_DATE_RANGE", ErrorType.Validation);
            }

            try
            {
                var rows = await appointmentClient.GetStatsDataAsync(from, to);

                var grouped = rows
                    .Where(r => r.Status == AppointmentStatus.Completed)
                    .GroupBy(r => r.DoctorId)
                    .ToDictionary(g => g.Key, g => (Revenue: g.Sum(r => r.TotalCost), Count: g.Count()));

                var names = await GetDoctorNamesAsync(grouped.Keys);

                var result = grouped
                    .Select(kv => new DoctorRevenueDto
                    {
                        DoctorId = kv.Key,
                        DoctorName = names[kv.Key],
                        Revenue = kv.Value.Revenue,
                        CompletedCount = kv.Value.Count
                    })
                    .OrderByDescending(x => x.Revenue)
                    .ToList();

                return Result<List<DoctorRevenueDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to build doctors revenue statistics");
                return Result<List<DoctorRevenueDto>>.Fail("Failed to load statistics data",
                    "UPSTREAM_FAILURE", ErrorType.UpstreamFailure);
            }
        }

        public async Task<Result<List<ProcedureProfitabilityDto>>> BuildProceduresProfitabilityAsync(DateOnly from, DateOnly to)
        {
            if (from > to)
            {
                return Result<List<ProcedureProfitabilityDto>>.Fail("From must not be after to",
                    "INVALID_DATE_RANGE", ErrorType.Validation);
            }

            try
            {
                var rows = await appointmentClient.GetStatsDataAsync(from, to);

                var result = rows
                    .Where(r => r.Status == AppointmentStatus.Completed)
                    .SelectMany(r => r.Procedures)
                    .GroupBy(p => p.ProcedureId)
                    .Select(g => new ProcedureProfitabilityDto
                    {
                        ProcedureId = g.Key,
                        ProcedureName = g.First().ProcedureName,
                        TimesPerformed = g.Count(),
                        Revenue = g.Sum(p => p.ProcedurePrice)
                    })
                    .OrderByDescending(x => x.Revenue)
                    .ToList();

                return Result<List<ProcedureProfitabilityDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to build procedures statistics");
                return Result<List<ProcedureProfitabilityDto>>.Fail("Failed to load statistics data",
                    "UPSTREAM_FAILURE", ErrorType.UpstreamFailure);
            }
        }

        public async Task<Result<PatientStatisticsDto>> BuildPatientStatisticsAsync(DateOnly from, DateOnly to)
        {
            if (from > to)
            {
                return Result<PatientStatisticsDto>.Fail("From must not be after to",
                    "INVALID_DATE_RANGE", ErrorType.Validation);
            }

            try
            {
                var rows = await appointmentClient.GetStatsDataAsync(from, to);
                var visits = rows.Where(r => r.Status != AppointmentStatus.Cancelled).ToList();
                var completed = rows.Where(r => r.Status == AppointmentStatus.Completed).ToList();

                var totalRevenue = completed.Sum(r => r.TotalCost);
                var payingPatients = completed.Select(r => r.PatientId).Distinct().Count();

                var top = visits
                    .GroupBy(r => r.PatientId)
                    .Select(g => new
                    {
                        PatientId = g.Key,
                        Visits = g.Count(),
                        Revenue = g.Where(r => r.Status == AppointmentStatus.Completed).Sum(r => r.TotalCost)
                    })
                    .OrderByDescending(x => x.Revenue)
                    .ThenByDescending(x => x.Visits)
                    .Take(5)
                    .ToList();

                var names = await GetPatientNamesAsync(top.Select(t => t.PatientId));

                var result = new PatientStatisticsDto
                {
                    TotalPatients = visits.Select(r => r.PatientId).Distinct().Count(),
                    TotalVisits = visits.Count,
                    TotalRevenue = totalRevenue,
                    AverageRevenuePerPatient = payingPatients == 0 ? 0 : Math.Round(totalRevenue / payingPatients, 2),
                    VisitsOverTime = visits
                        .GroupBy(r => r.DateTime.ToString("yyyy-MM"))
                        .OrderBy(g => g.Key)
                        .Select(g => new VisitsOverTimeDto { Period = g.Key, Visits = g.Count() })
                        .ToList(),
                    TopPatients = top.Select(t => new TopPatientDto
                    {
                        PatientId = t.PatientId,
                        PatientName = names[t.PatientId],
                        Visits = t.Visits,
                        Revenue = t.Revenue
                    }).ToList()
                };

                return Result<PatientStatisticsDto>.Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to build patient statistics");
                return Result<PatientStatisticsDto>.Fail("Failed to load statistics data",
                    "UPSTREAM_FAILURE", ErrorType.UpstreamFailure);
            }
        }

        private async Task<Dictionary<int, string>> GetDoctorNamesAsync(IEnumerable<int> ids)
        {
            var doctors = await Task.WhenAll(ids.Select(async id =>
                (Id: id, Doctor: await queryClient.GetDoctorAsync(id))));

            return doctors.ToDictionary(d => d.Id, d => d.Doctor != null
                ? $"{d.Doctor.FirstName} {d.Doctor.LastName}"
                : $"Unknown doctor ({d.Id})");
        }

        private async Task<Dictionary<int, string>> GetPatientNamesAsync(IEnumerable<int> ids)
        {
            var patients = await Task.WhenAll(ids.Select(async id =>
                (Id: id, Patient: await queryClient.GetPatientAsync(id))));

            return patients.ToDictionary(p => p.Id, p => p.Patient != null
                ? $"{p.Patient.Name} {p.Patient.LastName}"
                : $"Unknown patient ({p.Id})");
        }

        private static string MonthKey(DateTime date) =>
    date.ToString("yyyy-MM", CultureInfo.InvariantCulture);

        private static List<string> BuildMonthAxis(IEnumerable<DateTime> dates)
        {
            var list = dates.ToList();
            if (list.Count == 0)
            {
                return [];
            }

            var month = new DateTime(list.Min().Year, list.Min().Month, 1);
            var last = new DateTime(list.Max().Year, list.Max().Month, 1);
            var periods = new List<string>();

            for (; month <= last; month = month.AddMonths(1))
            {
                periods.Add(MonthKey(month));
            }

            return periods;
        }
    }
}