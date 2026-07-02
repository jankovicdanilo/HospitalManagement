using HospitalManagement.Data;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Repositories.Implementations
{
    public class ProcedureRepository : IProcedureRepository
    {
        private readonly HospitalDbContext dbContext;

        public ProcedureRepository(HospitalDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Procedure> CreateAsync(Procedure request)
        {
            await dbContext.Procedures.AddAsync(request);
            await dbContext.SaveChangesAsync();

            return request;
        }

        public async Task<Procedure?> DeleteAsync(int id)
        {
            var procedure = await dbContext.Procedures.FirstOrDefaultAsync(p => p.Id == id);

            if(procedure == null)
            {   
                return null;
            }

            dbContext.Procedures.Remove(procedure);
            await dbContext.SaveChangesAsync();

            return procedure;
        }

        public async Task<IEnumerable<Procedure>> GetAllAsync()
        {
            return await dbContext.Procedures.ToListAsync();
        }

        public async Task<Procedure?> GetByIdAsync(int id)
        {
            return await dbContext.Procedures.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Procedure?> UpdateAsync(int id, Procedure request)
        {
            var procedure = await dbContext.Procedures.FirstOrDefaultAsync(x =>x.Id == id);

            if(procedure == null)
            {
                return null;
            }

            procedure.Name = request.Name;
            procedure.Price = request.Price;

            await dbContext.SaveChangesAsync();

            return procedure;
        }
    }
}
