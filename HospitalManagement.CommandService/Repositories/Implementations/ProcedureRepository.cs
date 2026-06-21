using HospitalManagement.CommandService.Data;
using HospitalManagement.CommandService.Models.Domain;
using HospitalManagement.CommandService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.CommandService.Repositories.Implementations
{
    public class ProcedureRepository : IProcedureRepository
    {
        private readonly CommandDbContext dbContext;

        public ProcedureRepository(CommandDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Procedure> CreateAsync(Procedure request)
        {
            await dbContext.Procedures.AddAsync(request);
            await dbContext.SaveChangesAsync();
            return request;
        }

        public async Task<Procedure?> UpdateAsync(int id, Procedure request)
        {
            var procedure = await dbContext.Procedures.FirstOrDefaultAsync(x => x.Id == id);
            if (procedure == null) return null;
            procedure.Name = request.Name;
            procedure.Price = request.Price;
            await dbContext.SaveChangesAsync();
            return procedure;
        }

        public async Task<Procedure?> DeleteAsync(int id)
        {
            var procedure = await dbContext.Procedures.FirstOrDefaultAsync(p => p.Id == id);
            if (procedure == null) return null;
            dbContext.Procedures.Remove(procedure);
            await dbContext.SaveChangesAsync();
            return procedure;
        }
    }
}