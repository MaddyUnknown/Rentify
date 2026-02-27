using Rentify.DataAccess.Core.QueryResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.Core.Repositories
{
    public interface ITenantRepository
    {
        Task<IEnumerable<TenantSummaryQueryResult>> GetAllTenantSummaryAsync(int skipItems, int featchItems, DateTime asOfDate);

        Task<int> CountAsync(DateTime asOfDate);
    }
}
