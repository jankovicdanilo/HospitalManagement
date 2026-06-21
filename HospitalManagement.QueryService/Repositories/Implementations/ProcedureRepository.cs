using HospitalManagement.QueryService.Data;
using HospitalManagement.QueryService.Models.ReadModels;
using HospitalManagement.QueryService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.QueryService.Repositories.Implementations
{
    public class ProcedureRepository : IProcedureRepository
    {
        private readonly QueryDbContext dbContext;

        public ProcedureRepository(QueryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<ProcedureReadModel>> GetAllAsync()
        {
            return await dbContext.Procedures.AsNoTracking().ToListAsync();
        }

        public async Task<ProcedureReadModel?> GetByIdAsync(int id)
        {
            return await dbContext.Procedures.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}