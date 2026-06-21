using HospitalManagement.QueryService.Models.ReadModels;

namespace HospitalManagement.QueryService.Repositories.Interfaces
{
    public interface IDoctorScheduleRepository
    {
        Task<DoctorScheduleReadModel?> GetByIdAsync(int id);
        Task<DoctorScheduleReadModel?> GetByDoctorIdAndDayAsync(int doctorId, DayOfWeek dayOfWeek);
        Task<List<DoctorScheduleReadModel>> GetAllByDoctorIdAsync(int doctorId);
        Task<bool> DoctorExists(int id);
    }
}