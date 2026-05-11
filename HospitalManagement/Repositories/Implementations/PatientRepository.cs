using HospitalManagement.Data;
using HospitalManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Repositories.Implementations
{
    public class PatientRepository : IPatientRepository
    {
        private readonly HospitalDbContext dbContext;

        public PatientRepository(HospitalDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Delete(int id)
        {
            var patient = await dbContext.Patients.FirstOrDefaultAsync(x => x.Id == id);

            dbContext.Patients.Remove(patient);
            dbContext.SaveChanges();
        }
    }
}
