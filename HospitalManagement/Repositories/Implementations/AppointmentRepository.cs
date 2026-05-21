using HospitalManagement.Data;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.Enums;
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

        

        public async Task<List<Appointment>?> GetByDoctorIdAsync(int id)
        {
            return await dbContext.Appointments.AsNoTracking().Where(x => x.DoctorId == id).ToListAsync();
        }

        public async Task<Appointment> UpdateAsync(Appointment appointment)
        {
            dbContext.Appointments.Update(appointment);
            await dbContext.SaveChangesAsync();

            return appointment;
        }

        public async Task<List<Appointment>> GetByDoctorIdAndDateAsync(int doctorId, DateOnly date)
        {
            return await dbContext.Appointments.Where(x => x.DoctorId == doctorId && 
                DateOnly.FromDateTime(x.DateTime) == date)
                .ToListAsync();
        }

        public async Task<Appointment> CreateAsync(Appointment appointment)
        {
            await dbContext.Appointments.AddAsync(appointment);
            await dbContext.SaveChangesAsync();

            return appointment;
        }

        public async Task<List<Appointment>> GetAllAsync()
        {
            return await dbContext.Appointments.AsNoTracking().Include(x => x.Doctor).Include(x => x.Patient).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetPendingPastAppointmentsAsync()
        {
            return await dbContext.Appointments.Where(a => a.Status == AppointmentStatus.Pending && a.DateTime < DateTime.UtcNow).ToListAsync();
        }
    }
}
