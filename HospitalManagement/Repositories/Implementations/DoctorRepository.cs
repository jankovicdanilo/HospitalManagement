using HospitalManagement.Data;
using HospitalManagement.Models.Domain;
using HospitalManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Repositories.Implementations
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly HospitalDbContext dbContext;

        public DoctorRepository(HospitalDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Doctor?> CreateAsync(Doctor request)
        {
            await dbContext.Doctors.AddAsync(request);

            await dbContext.SaveChangesAsync();

            return request;
        }

        public async Task Delete(int id)
        {
            var doctor = await dbContext.Doctors.FirstOrDefaultAsync(x => x.Id == id);

            dbContext.Doctors.Remove(doctor!);

            await dbContext.SaveChangesAsync();
        }

        public async Task<List<Doctor>> GetAllAsync()
        {
            return await dbContext.Doctors.AsNoTracking().ToListAsync();
        }

        public async Task<Doctor?> GetByIdAsync(int id)
        {
            return await dbContext.Doctors.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Doctor?> UpdateAsync(Doctor request)
        {
            dbContext.Doctors.Update(request);

            await dbContext.SaveChangesAsync();

            return request;
        }
    }
}
