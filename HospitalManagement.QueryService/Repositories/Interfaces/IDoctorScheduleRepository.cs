using HospitalManagement.Shared.Models.Domain;

namespace HospitalManagement.QueryService.Repositories.Interfaces
{
    public interface IDoctorScheduleRepository
    {
        Task<DoctorSchedule?> GetByIdAsync(int id);
        Task<DoctorSchedule?> GetByDoctorIdAndDayAsync(int doctorId, DayOfWeek dayOfWeek);
        Task<List<DoctorSchedule>> GetAllByDoctorIdAsync(int doctorId);
        Task<bool> DoctorExists(int id);
    }
}