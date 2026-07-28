using HospitalManagement.CommandService.Repositories.Interfaces;
using HospitalManagement.Shared.Data;
using HospitalManagement.Shared.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace HospitalManagement.CommandService.Repositories.Implementations
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly HospitalManagementDbContext dbContext;

        public DoctorRepository(HospitalManagementDbContext dbContext)
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

        public async Task<Doctor?> GetByEmailAsync(string email)
        {
            return await dbContext.Doctors
                .FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}