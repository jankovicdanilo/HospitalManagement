using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Appointment;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Appointment?> Delete(int id);

        Task<Appointment?> GetByIdAsync(int id); 

        Task<Appointment> CreateAsync(Appointment appointment);

        Task<PagedResult<AppointmentListResponseDto>> GetAllAsync(AppointmentFilterDto filer);

        Task<Appointment> UpdateAsync(Appointment appointment);

        Task<List<Appointment>> GetByDoctorIdAndDateAsync(int doctorId, DateOnly date);

        Task<List<Appointment>?> GetByDoctorIdAsync(int id);

        Task<IEnumerable<Appointment>> GetPendingPastAppointmentsAsync();
    }
}
