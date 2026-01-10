using Rentify.Core.Entities;
using Rentify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.Core.Repositories
{
    public interface IMediaFileVariantRepository
    {
        Task<IEnumerable<MediaFileVariant>> GetAllByMediaFileIdAsync(int mediaFileId);
        Task<MediaFileVariant?> GetByMediaFileIdAndVariantTypeAsync(int mediaFileId, MediaFileVariantEnum variantType);
    }
}
