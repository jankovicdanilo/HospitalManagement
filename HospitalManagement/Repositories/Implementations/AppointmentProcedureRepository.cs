using HospitalManagement.Data;
using HospitalManagement.Models.Domain;
using HospitalManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Repositories.Implementations
{
    public class AppointmentProcedureRepository : IAppointmentProcedureRepository
    {
        private readonly HospitalDbContext dbContext;

        public AppointmentProcedureRepository(HospitalDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AppointmentProcedure?> AddAsync(AppointmentProcedure request)
        {
            await dbContext.AppointmentProcedures.AddAsync(request);
            await dbContext.SaveChangesAsync();

            await dbContext.Entry(request).Reference(x => x.Procedure).LoadAsync();

            return request;
        }

        public async Task<AppointmentProcedure?> GetAsync(int appointmentId, int procedureId)
        {
            return await dbContext.AppointmentProcedures
                .Include(a => a.Procedure)
                .FirstOrDefaultAsync(x => x.AppointmentId == appointmentId && x.ProcedureId == procedureId);
        }

        public async Task<AppointmentProcedure?> RemoveAsync(int appointmentId, int procedureId)
        {
            var appointmentProcedure = await dbContext.AppointmentProcedures
                .FirstOrDefaultAsync
                (x => x.AppointmentId == appointmentId && x.ProcedureId == procedureId);

            if(appointmentProcedure is null)
            {
                return null;
            }

            dbContext.AppointmentProcedures.Remove(appointmentProcedure);
            await dbContext.SaveChangesAsync();

            return appointmentProcedure;
        }
    }
}
