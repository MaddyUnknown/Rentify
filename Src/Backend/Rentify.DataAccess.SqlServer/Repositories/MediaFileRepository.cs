using Microsoft.EntityFrameworkCore;
using Rentify.Core.Entities;
using Rentify.Core.Enums;
using Rentify.DataAccess.Core.Options;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.SqlServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.SqlServer.Repositories
{
    public class MediaFileRepository : IMediaFileRepository
    {
        private readonly RentifyDbContext _context;

        public MediaFileRepository(RentifyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MediaFile>> GetMediaFilesByEntityAsync(MediaFileEntityEnum entityType, int entityId, MediaFileFilterOption? options = null)
        {
            options ??= MediaFileFilterOption.Empty;

            return await _context.MediaFiles.Include(f => f.MediaFileLink).Include(f => f.MediaFileVariants)
                .Where(f => f.MediaFileLink.EntityType == entityType && f.MediaFileLink.EntityId == entityId && (options.FilterDeletedRecords == false || f.Status != MediaFileStatusEnum.Deleted))
                .ToListAsync();
        }

        public async Task<MediaFile?> GetMediaFileByIdAsync(int id)
        {
            return await _context.MediaFiles
                .Where(f => f.Id == id)
                .Include(f => f.MediaFileLink)
                .Include(f => f.MediaFileVariants)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<MediaFile>> GetMediaFileByIdsAsync(IEnumerable<int> ids)
        {
            return await _context.MediaFiles
                .Where(f => ids.Contains(f.Id))
                .Include(f => f.MediaFileLink)
                .Include(f => f.MediaFileVariants)
                .ToListAsync();
        }
    }
}
