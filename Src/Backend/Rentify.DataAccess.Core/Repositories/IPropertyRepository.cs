using Rentify.Core.Entities;
using Rentify.Core.Enums;
using Rentify.DataAccess.Core.QueryResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.Core.Repositories
{
    public interface IPropertyRepository
    {
        Task<IEnumerable<PropertySummaryQueryResult>> GetAllPropertySummaryAsync(int skipItems, int featchItems, DateTime asOfDate);
        Task<int> CountAsync(DateTime asOfDate);
        Task<bool> UpdateRequestedCoverPicAsync(int id, int mediaFileId);
        Task<bool> CommitActiveCoverPicAsync(int id, int mediaFileId);
    }
}
