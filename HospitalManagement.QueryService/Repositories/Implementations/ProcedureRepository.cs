using HospitalManagement.Shared.Data;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.QueryService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using HospitalManagement.Shared.Extensions;

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
            return await dbContext.Procedures.AsNoTracking().ToPagedResultAsync(x => x.Id, pageNumber, pageSize);
        }

        public async Task<Procedure?> GetByIdAsync(int id)
        {
            return await dbContext.Procedures.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}