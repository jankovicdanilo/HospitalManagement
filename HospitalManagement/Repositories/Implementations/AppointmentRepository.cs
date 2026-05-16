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

        public async Task<List<Appointment>?> GetByDoctorIdAsync(int id)
        {
            return await dbContext.Appointments.Where(x => x.DoctorId == id).ToListAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await dbContext.Appointments.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Appointment> UpdateAsync(Appointment appointment)
        {
            dbContext.Appointments.Update(appointment);
            await dbContext.SaveChangesAsync();

            return appointment;
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
    }
}
