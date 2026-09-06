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

        public async Task<Treatment> CreateAsync(Treatment treatment)
        {
            dbContext.Treatments.Add(treatment);
            await dbContext.SaveChangesAsync();
            return treatment;
        }

        public async Task<List<Treatment>> GetByAppointmentIdsAsync(IEnumerable<int> appointmentIds)
        {
            return await dbContext.Treatments
                .Where(t => appointmentIds.Contains(t.AppointmentId))
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Treatment?> GetByIdAsync(int id)
        {
            return await dbContext.Treatments.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<bool> TreatmentExists(int appointmentId)
        {
            return await dbContext.Treatments.AnyAsync(x => x.AppointmentId == appointmentId);
        }
    }
}
