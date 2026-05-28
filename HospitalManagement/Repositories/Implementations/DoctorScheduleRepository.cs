using HospitalManagement.Data;
using HospitalManagement.Models.Domain;
using HospitalManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Repositories.Implementations
{
    public class DoctorScheduleRepository : IDoctorScheduleRepository
    {
        private readonly HospitalDbContext dbContext;

        public DoctorScheduleRepository(HospitalDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<DoctorSchedule?> CreateAsync(DoctorSchedule doctorSchedule)
        {
            await dbContext.DoctorSchedules.AddAsync(doctorSchedule);
            await dbContext.SaveChangesAsync();
            return doctorSchedule;
        }

        public async Task<DoctorSchedule?> Delete(int id)
        {
            var doctorSchedule = await dbContext.DoctorSchedules.FirstOrDefaultAsync(x => x.Id == id);

            if(doctorSchedule == null)
            {
                return null;
            }

            dbContext.DoctorSchedules.Remove(doctorSchedule);
            await dbContext.SaveChangesAsync();

            return doctorSchedule;
        }

        public async Task<bool> DoctorExists(int id)
        {
            return await dbContext.Doctors.AnyAsync(x => x.Id == id);
        }

        public async Task<List<DoctorSchedule>> GetAllByDoctorIdAsync(int doctorId)
        {
            return await dbContext.DoctorSchedules.Where(x => x.DoctorId == doctorId).ToListAsync();
        }

        public async Task<DoctorSchedule?> GetByDoctorIdAndDayAsync(int doctorId, DayOfWeek dayOfWeek)
        {
            return await dbContext.DoctorSchedules.Where(x => x.DoctorId == doctorId &&  x.DayOfWeek == dayOfWeek).FirstOrDefaultAsync();
        }

        public async Task<DoctorSchedule?> GetByIdAsync(int id)
        {
            return await dbContext.DoctorSchedules.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<DoctorSchedule> UpdateAsync(DoctorSchedule doctorSchedule)
        {
            dbContext.DoctorSchedules.Update(doctorSchedule);
            await dbContext.SaveChangesAsync();
            return doctorSchedule;
        }
    }
}
