using HospitalManagement.Appointments.Data;
using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Appointments.Repositories.Implementations
{
    public class TreatmentRepository : ITreatmentRepository
    {
        private readonly AppointmentDbContext dbContext;

        public TreatmentRepository(AppointmentDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<Treatment> CreateAsync(Treatment treatment)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Treatment>> GetByAppointmentIdsAsync(IEnumerable<int> appointmentIds)
        {
            return await dbContext.Treatments
                .Where(t => appointmentIds.Contains(t.AppointmentId))
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();
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
