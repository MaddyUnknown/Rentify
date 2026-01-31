using Rentify.Core.Entities;
using Rentify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.Core.Repositories
{
    public interface IMediaFileRepository
    {
        Task<IEnumerable<MediaFile>> GetMediaFilesByEntityAsync(MediaFileEntityEnum entityType, int entityId, bool filterDeletedRecords = false);
        Task<MediaFile?> GetMediaFileByIdAsync(int id);
        Task<IEnumerable<MediaFile>> GetMediaFileByIdsAsync(IEnumerable<int> ids);

    }
}
