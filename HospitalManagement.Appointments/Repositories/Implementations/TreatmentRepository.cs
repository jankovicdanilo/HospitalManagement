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
            treatment.CreatedAt = DateTime.UtcNow;
            await dbContext.AddAsync(treatment);
            await dbContext.SaveChangesAsync();
            return treatment;
        }

        public async Task<Treatment?> GetByIdAsync(int id)
        {
            return await dbContext.Treatments.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> TreatmentExists(int appointmentId)
        {
            return await dbContext.Treatments.AnyAsync(x => x.AppointmentId == appointmentId);
        }
    }
}