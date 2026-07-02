using HospitalManagement.Shared.Data;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.QueryService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.QueryService.Repositories.Implementations
{
    public class DoctorScheduleRepository : IDoctorScheduleRepository
    {
        private readonly HospitalManagementDbContext dbContext;

        public DoctorScheduleRepository(HospitalManagementDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<DoctorSchedule?> GetByIdAsync(int id)
        {
            return await dbContext.DoctorSchedules.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<DoctorSchedule?> GetByDoctorIdAndDayAsync(int doctorId, DayOfWeek dayOfWeek)
        {
            return await dbContext.DoctorSchedules.AsNoTracking()
                .Where(x => x.DoctorId == doctorId && x.DayOfWeek == dayOfWeek)
                .FirstOrDefaultAsync();
        }

        public async Task<List<DoctorSchedule>> GetAllByDoctorIdAsync(int doctorId)
        {
            return await dbContext.DoctorSchedules.AsNoTracking()
                .Where(x => x.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<bool> DoctorExists(int id)
        {
            return await dbContext.Doctors.AnyAsync(x => x.Id == id);
        }
    }
}