using HospitalManagement.Common;

namespace HospitalManagement.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<Result> Delete(int id);
    }
}
