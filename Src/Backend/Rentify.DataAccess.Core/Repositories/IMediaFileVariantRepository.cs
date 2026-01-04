using Rentify.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.Core.Repositories
{
    public interface IMediaFileVariantRepository
    {
        Task<IEnumerable<MediaFileVariant>> GetMediaFileByMediaFileIdAsync(int mediaFileId);
    }
}
