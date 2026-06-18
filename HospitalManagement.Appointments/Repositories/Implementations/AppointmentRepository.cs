using HospitalManagement.Shared.Common;
using HospitalManagement.Appointments.Data;
using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Models.DTOs.Appointment;
using HospitalManagement.Appointments.Models.Enums;
using HospitalManagement.Appointments.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Appointments.Repositories.Implementations
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppointmentDbContext dbContext;

        public AppointmentRepository(AppointmentDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await dbContext.Appointments
                .Include(x => x.AppointmentProcedures)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Appointment?> Delete(int id)
        {
            var appointment = await dbContext.Appointments.FirstOrDefaultAsync(x => x.Id == id);

            if (appointment == null)
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

        public async Task<(List<Appointment> items, int totalCount)> GetAllAsync(AppointmentFilterDto filter)
        {
            var query = dbContext.Appointments
                .Include(x => x.AppointmentProcedures)
                .AsQueryable();

            if (filter.DoctorId.HasValue)
            {
                query = query.Where(x => x.DoctorId == filter.DoctorId.Value);
            }

            if (filter.PatientId.HasValue)
            {
                query = query.Where(x => x.PatientId == filter.PatientId.Value);
            }

            if (filter.Date.HasValue)
            {
                query = query.Where(x => DateOnly.FromDateTime(x.DateTime) == filter.Date.Value);
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(x => x.Status == filter.Status.Value);
            }

            var totalCount = await query.CountAsync();
            var offset = (filter.PageNumber - 1) * filter.PageSize;

            var items = await query
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(filter.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<Appointment>> GetPendingPastAppointmentsAsync()
        {
            var now = DateTime.UtcNow;
            var appointments = await dbContext.Appointments
                .Where(a => a.Status == AppointmentStatus.Pending && a.DateTime < now)
                .ToListAsync();

            return appointments.Where(a => a.DateTime.Add(a.Duration).AddHours(1) < now);
        }
    }
}