using HospitalManagement.CommandService.Data;
using HospitalManagement.CommandService.Models.Domain;
using HospitalManagement.CommandService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.CommandService.Repositories.Implementations
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly CommandDbContext dbContext;

        public DoctorRepository(CommandDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Doctor?> CreateAsync(Doctor request)
        {
            await dbContext.Doctors.AddAsync(request);
            await dbContext.SaveChangesAsync();
            return request;
        }

        public async Task<Doctor?> UpdateAsync(Doctor request)
        {
            dbContext.Doctors.Update(request);
            await dbContext.SaveChangesAsync();
            return request;
        }

        public async Task Delete(int id)
        {
            var doctor = await dbContext.Doctors.FirstOrDefaultAsync(x => x.Id == id);
            dbContext.Doctors.Remove(doctor!);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Doctor?> GetByIdAsync(int id)
        {
            return await dbContext.Doctors.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}