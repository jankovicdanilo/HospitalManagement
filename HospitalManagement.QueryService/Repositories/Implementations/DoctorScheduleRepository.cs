using HospitalManagement.QueryService.Data;
using HospitalManagement.QueryService.Models.ReadModels;
using HospitalManagement.QueryService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.QueryService.Repositories.Implementations
{
    public class DoctorScheduleRepository : IDoctorScheduleRepository
    {
        private readonly QueryDbContext dbContext;

        public DoctorScheduleRepository(QueryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<DoctorScheduleReadModel?> GetByIdAsync(int id)
        {
            return await dbContext.DoctorSchedules.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<DoctorScheduleReadModel?> GetByDoctorIdAndDayAsync(int doctorId, DayOfWeek dayOfWeek)
        {
            return await dbContext.DoctorSchedules.AsNoTracking()
                .Where(x => x.DoctorId == doctorId && x.DayOfWeek == dayOfWeek)
                .FirstOrDefaultAsync();
        }

        public async Task<List<DoctorScheduleReadModel>> GetAllByDoctorIdAsync(int doctorId)
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