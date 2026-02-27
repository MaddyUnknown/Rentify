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
    public class TenantEmergencyContactRepository : ITenantEmergencyContactRepository
    {
        private readonly RentifyDbContext _context;

        public TenantEmergencyContactRepository(RentifyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TenantEmergencyContact>> GetAllByTenantId(int tenantId)
        {
            return await _context.TenantEmergencyContacts.Where(p => p.TenantId <= tenantId).ToListAsync();
        }
    }
}
