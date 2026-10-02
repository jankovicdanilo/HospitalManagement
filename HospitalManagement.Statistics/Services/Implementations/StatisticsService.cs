using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Models.DTOs.Statistics;
using HospitalManagement.Statistics.Services.Interfaces;
using HospitalManagement.Statistics.Services.Utility;

namespace HospitalManagement.Statistics.Services.Implementations
{
    public class StatisticsService : IStatisticsService
    {
        private readonly StatisticsBuilder builder;
        private readonly StatisticsCache cache;

        public StatisticsService(StatisticsBuilder builder, StatisticsCache cache)
        {
            this.builder = builder;
            this.cache = cache;
        }

        public Task<Result<List<DoctorLoadTimelineDto>>> GetDoctorsLoadTimelineAsync(DateOnly from, DateOnly to, int top)
    => cache.GetOrSetAsync($"doctors-load-timeline-{top}", from, to,
        () => builder.BuildDoctorsLoadTimelineAsync(from, to, top));

        public Task<Result<DoctorStatisticsDto>> GetDoctorStatisticsAsync(int doctorId, DateOnly from, DateOnly to)
            => cache.GetOrSetAsync($"doctor-{doctorId}", from, to,
                () => builder.BuildDoctorStatisticsAsync(doctorId, from, to));

        public Task<Result<List<DoctorLoadDto>>> GetDoctorsLoadAsync(DateOnly from, DateOnly to)
            => cache.GetOrSetAsync("doctors-load", from, to, () => builder.BuildDoctorsLoadAsync(from, to));

        public Task<Result<List<DoctorRevenueDto>>> GetDoctorsRevenueAsync(DateOnly from, DateOnly to)
            => cache.GetOrSetAsync("doctors-revenue", from, to, () => builder.BuildDoctorsRevenueAsync(from, to));

        public Task<Result<List<ProcedureProfitabilityDto>>> GetProceduresProfitabilityAsync(DateOnly from, DateOnly to)
            => cache.GetOrSetAsync("procedures-profitability", from, to, () => builder.BuildProceduresProfitabilityAsync(from, to));

        public Task<Result<PatientStatisticsDto>> GetPatientStatisticsAsync(DateOnly from, DateOnly to)
            => cache.GetOrSetAsync("patients", from, to, () => builder.BuildPatientStatisticsAsync(from, to));
    }
}