using Microsoft.EntityFrameworkCore;
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
    public class MediaFileRepository : IMediaFileRepository
    {
        private readonly RentifyDbContext _context;

        public MediaFileRepository(RentifyDbContext context)
        {
            _context = context;
        }

        public async Task<MediaFileLink?> GetCoverMediaFileLinkByEntityAsync(MediaFileEntityEnum entityType, int entityId)
        {
            return await _context.MediaFileLinks
                .Where(l => l.MarkedAsCover == true)
                .Include(l => l.MediaFile)
                .ThenInclude(m => m.MediaFileVariants)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<MediaFile>> GetMediaFilesByEntityAsync(MediaFileEntityEnum entityType, int entityId, bool filterDeletedRecords = false)
        {
            return await _context.MediaFileLinks
                .Where(l => l.EntityType == entityType && l.EntityId == entityId && (filterDeletedRecords == false || l.MediaFile.Status != MediaFileStatusEnum.Deleted))
                .Include(l => l.MediaFile).ThenInclude(f => f.MediaFileLink)
                .Include(l => l.MediaFile).ThenInclude(f => f.MediaFileVariants)
                .Select(l => l.MediaFile)
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
