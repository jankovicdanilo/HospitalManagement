using HospitalManagement.Appointments.Data;
using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Appointments.Repositories.Implementations
{
    public class AppointmentProcedureRepository : IAppointmentProcedureRepository
    {
        private readonly AppointmentDbContext dbContext;

        public AppointmentProcedureRepository(AppointmentDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AppointmentProcedure?> CreateAsync(AppointmentProcedure request)
        {
            await dbContext.AppointmentProcedures.AddAsync(request);
            await dbContext.SaveChangesAsync();

            return request;
        }

        public async Task<AppointmentProcedure?> GetByAppointmentAndProcedureIdAsync(int appointmentId, int procedureId)
        {
            return await dbContext.AppointmentProcedures
                .FirstOrDefaultAsync(x => x.AppointmentId == appointmentId && x.ProcedureId == procedureId);
        }

        public async Task<AppointmentProcedure?> DeleteAsync(int appointmentId, int procedureId)
        {
            var appointmentProcedure = await dbContext.AppointmentProcedures
                .FirstOrDefaultAsync
                (x => x.AppointmentId == appointmentId && x.ProcedureId == procedureId);

            if (appointmentProcedure is null)
            {
                return null;
            }

            dbContext.AppointmentProcedures.Remove(appointmentProcedure);
            await dbContext.SaveChangesAsync();

            return appointmentProcedure;
        }
    }
}