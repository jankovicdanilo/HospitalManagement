using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Shared.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<(List<T> items, int totalCount)> ToPagedResultAsync<T, TKey>(
            this IQueryable<T> query,
            Expression<Func<T, TKey>> orderBy,
            int pageNumber,
            int pageSize)
        {
            var orderedQuery = query.OrderBy(orderBy);

            var totalCount = await orderedQuery.CountAsync();
            var offset = (pageNumber - 1) * pageSize;

            var items = await orderedQuery.Skip(offset).Take(pageSize).ToListAsync();

            return (items, totalCount);
        }
    }
}
