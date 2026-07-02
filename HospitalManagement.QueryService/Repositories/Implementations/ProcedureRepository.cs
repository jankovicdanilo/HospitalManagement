using HospitalManagement.Shared.Data;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.QueryService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.QueryService.Repositories.Implementations
{
    public class ProcedureRepository : IProcedureRepository
    {
        private readonly HospitalManagementDbContext dbContext;

        public ProcedureRepository(HospitalManagementDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Procedure>> GetAllAsync()
        {
            return await dbContext.Procedures.AsNoTracking().ToListAsync();
        }

        public async Task<Procedure?> GetByIdAsync(int id)
        {
            return await dbContext.Procedures.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}