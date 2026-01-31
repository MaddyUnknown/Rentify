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
    public class MediaFileLinkRepository : IMediaFileLinkRepository
    {
        private readonly RentifyDbContext _context;

        public MediaFileLinkRepository(RentifyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CommitRequestedCoverForEntityAsync(int id, MediaFileEntityEnum entityType, int entityId)
        {
            int rowsAffected = await _context.Database.ExecuteSqlAsync($"UPDATE [MediaFileLinks] SET MarkedAsCover = CASE WHEN Id = {id} AND MarkAsCoverRequested = 1 THEN 1 ELSE 0 END WHERE EntityId = {entityId} AND EntityType = {entityType}");
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateRequestedCoverForEntityAsync(int id, MediaFileEntityEnum entityType, int entityId)
        {
            int rowsAffected = await _context.Database.ExecuteSqlAsync($"UPDATE [MediaFileLinks] SET MarkAsCoverRequested = CASE WHEN Id = {id} THEN 1 ELSE 0 END WHERE EntityId = {entityId} AND EntityType = {entityType}");
            return rowsAffected > 0;
        }
    }
}
