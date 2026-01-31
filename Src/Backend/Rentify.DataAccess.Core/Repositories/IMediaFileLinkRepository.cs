using Rentify.Core.Entities;
using Rentify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.Core.Repositories
{
    public interface IMediaFileLinkRepository
    {
        Task<bool> CommitRequestedCoverForEntityAsync(int id, MediaFileEntityEnum entityType, int entityId);
        Task<bool> UpdateRequestedCoverForEntityAsync(int id, MediaFileEntityEnum entityType, int entityId);
    }
}
