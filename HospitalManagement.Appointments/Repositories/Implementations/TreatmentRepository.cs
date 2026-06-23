using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Repositories.Interfaces;

namespace HospitalManagement.Appointments.Repositories.Implementations
{
    public class TreatmentRepository : ITreatmentRepository
    {
        public Task<Treatment> CreateAsync(Treatment treatment)
        {
            throw new NotImplementedException();
        }

        public Task<Treatment?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TreatmentExists(int appointmentId)
        {
            throw new NotImplementedException();
        }
    }
}
