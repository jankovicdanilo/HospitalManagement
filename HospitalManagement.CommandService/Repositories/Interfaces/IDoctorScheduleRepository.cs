using HospitalManagement.Shared.Models.Domain;

namespace HospitalManagement.CommandService.Repositories.Interfaces
{
    public interface IDoctorScheduleRepository
    {
        Task<DoctorSchedule?> CreateAsync(DoctorSchedule doctorSchedule);
        Task<DoctorSchedule?> UpdateAsync(DoctorSchedule doctorSchedule);
        Task<DoctorSchedule?> Delete(int id);
        Task<bool> DoctorExists(int id);
        Task<DoctorSchedule?> GetByDoctorIdAndDayAsync(int doctorId, DayOfWeek dayOfWeek);
        Task<DoctorSchedule?> GetByIdAsync(int id);
    }
}