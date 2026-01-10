using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Rentify.Core.Entities;
using Rentify.Core.Enums;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.SqlServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.SqlServer.Repositories
{
    public class MediaFileVariantRepository : IMediaFileVariantRepository
    {
        private readonly RentifyDbContext _context;

        public MediaFileVariantRepository(RentifyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MediaFileVariant>> GetAllByMediaFileIdAsync(int mediaFileId)
        {
            return await _context.MediaFileVariants.Where(m => m.MediaFileId == mediaFileId).ToListAsync();
        }

        public async Task<MediaFileVariant?> GetByMediaFileIdAndVariantTypeAsync(int mediaFileId, MediaFileVariantEnum variantType)
        {
            return await _context.MediaFileVariants.Where(m => m.MediaFileId == mediaFileId && m.VariantType == variantType).FirstOrDefaultAsync();
        }
    }
}
