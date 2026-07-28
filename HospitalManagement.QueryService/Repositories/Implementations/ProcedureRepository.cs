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

        public async Task<(IEnumerable<Procedure> items, int totalCount)> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = dbContext.Procedures.AsNoTracking().OrderBy(x => x.Id);

            var totalCount = await query.CountAsync();
            var offset = (pageNumber - 1) * pageSize;

            var items = await query.Skip(offset).Take(pageSize).ToListAsync();

            return (items, totalCount);
        }

        public async Task<Procedure?> GetByIdAsync(int id)
        {
            return await dbContext.Procedures.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}