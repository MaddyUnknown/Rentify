using Microsoft.EntityFrameworkCore;
using Rentify.Core.Entities;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.SqlServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.SqlServer.Repositories
{
    public class UnitRepository : IUnitRepository
    {
        private readonly RentifyDbContext _context;

        public UnitRepository(RentifyDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountByPropertyIdAsync(int propertyId)
        {
            return await _context.Units.Where(p => p.PropertyId == propertyId).CountAsync();
        }

        public async Task<IEnumerable<Unit>> GetByPropertyIdAsync(int propertyId)
        {
            return await _context.Units.Where(p => p.PropertyId == propertyId).ToListAsync();
        }
    }
}
