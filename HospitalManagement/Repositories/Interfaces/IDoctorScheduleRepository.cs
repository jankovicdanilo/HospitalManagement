using HospitalManagement.Models.Domain;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface IDoctorScheduleRepository
    {
        Task<DoctorSchedule?> GetByIdAsync(int id);
        Task<DoctorSchedule?> GetByDoctorIdAndDayAsync(int doctorId, DayOfWeek dayOfWeek);
        Task<List<DoctorSchedule>> GetAllByDoctorIdAsync(int doctorId);
        Task<DoctorSchedule?> CreateAsync(DoctorSchedule doctorSchedule);
        Task<DoctorSchedule?> Delete(int id);
        Task<DoctorSchedule> UpdateAsync(DoctorSchedule doctorSchedule);
        Task<bool> DoctorExists(int id);
    }
}
