using HospitalManagement.Data;
using HospitalManagement.Models.Domain;
using HospitalManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Repositories.Implementations
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly HospitalDbContext dbContext;

        public AppointmentRepository(HospitalDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await dbContext.Appointments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Appointment?> Delete(int id)
        {
            var appointment = await dbContext.Appointments.FirstOrDefaultAsync(x => x.Id == id);

            if(appointment == null)
            {
                return null;
            }

            dbContext.Appointments.Remove(appointment);
            await dbContext.SaveChangesAsync();

            return appointment;
        }

        
    }
}
