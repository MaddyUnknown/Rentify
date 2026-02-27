using Microsoft.EntityFrameworkCore;
using Rentify.Core.Enums;
using Rentify.DataAccess.Core.QueryResults;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.SqlServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.SqlServer.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly RentifyDbContext _context;

        public TenantRepository(RentifyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TenantSummaryQueryResult>> GetAllTenantSummaryAsync(int skipItems, int featchItems, DateTime asOfDate)
        {
            return await _context
                .Tenants.Where(p => p.CreatedDate <= asOfDate)
                .Skip(skipItems)
                .Take(featchItems)
                .Select(t => new TenantSummaryQueryResult
                {
                    Id = t.Id,
                    Name = t.Name,
                    Email = t.Email,
                    PhoneNumber = t.PhoneNumber
                })
                .AsNoTracking()
                .ToListAsync();
        }
        
        public async Task<int> CountAsync(DateTime asOfDate)
        {
            return await _context.Tenants.Where(t => t.CreatedDate <= asOfDate).CountAsync();
        }
    }
}
