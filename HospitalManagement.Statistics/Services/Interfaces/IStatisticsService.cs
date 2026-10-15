using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Models.DTOs.Statistics;

namespace HospitalManagement.Statistics.Services.Interfaces
{
    public interface IStatisticsService
    {
        Task<Result<List<DoctorRevenueDto>>> GetDoctorsRevenueAsync(DateOnly from, DateOnly to);
        Task<Result<List<DoctorLoadDto>>> GetDoctorsLoadAsync(DateOnly from, DateOnly to);
        Task<Result<List<ProcedureProfitabilityDto>>> GetProceduresProfitabilityAsync(DateOnly from, DateOnly to);
        Task<Result<PatientStatisticsDto>> GetPatientStatisticsAsync(DateOnly from, DateOnly to);
        Task<Result<List<DoctorLoadTimelineDto>>> GetDoctorsLoadTimelineAsync(DateOnly from, DateOnly to, int top);
        Task<Result<DoctorStatisticsDto>> GetDoctorStatisticsAsync(int doctorId, DateOnly from, DateOnly to);
    }
}
