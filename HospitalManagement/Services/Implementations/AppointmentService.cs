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
    }
}
