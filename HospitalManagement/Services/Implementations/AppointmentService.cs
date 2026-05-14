using HospitalManagement.Common;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;

namespace HospitalManagement.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository apointmentRepository;

        public AppointmentService(IAppointmentRepository apointmentRepository)
        {
            this.apointmentRepository = apointmentRepository;
        }

        public async Task<Result> Delete(int id)
        {
            var appointmentDomain = await apointmentRepository.Delete(id);

            if (appointmentDomain == null)
            {
                return Result.Fail($"Appointment with the id {id} not found", "INVALID_ID");
            }

            return Result.Ok("Appointment deleted");
        }
    }
}
