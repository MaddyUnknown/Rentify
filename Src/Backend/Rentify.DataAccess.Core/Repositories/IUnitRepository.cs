using Rentify.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.Core.Repositories
{
    public interface IUnitRepository
    {
        Task<IEnumerable<Unit>> GetByPropertyIdAsync(int propertyId);
        Task<int> CountByPropertyIdAsync(int propertyId);
    }
}
