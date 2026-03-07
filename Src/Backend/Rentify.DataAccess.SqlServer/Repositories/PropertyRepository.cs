using Microsoft.EntityFrameworkCore;
using Rentify.Core.Entities;
using Rentify.Core.Enums;
using Rentify.DataAccess.Core.QueryResults;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.SqlServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.SqlServer.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly RentifyDbContext _context;

        public PropertyRepository(RentifyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PropertySummaryQueryResult>> GetAllPropertySummaryAsync(int skipItems, int featchItems, DateTime asOfDate)
        {
            return await _context
                .Properties.Where(p => p.CreatedDate <= asOfDate)
                .Include(p => p.ActiveCoverPic).ThenInclude(m => m!.MediaFileVariants)
                .OrderBy(p => p.Id)
                .Skip(skipItems)
                .Take(featchItems)
                .Select(p => new PropertySummaryQueryResult
                {
                    Id = p.Id,
                    Name = p.Name,
                    Address = p.PropertyAddress,
                    NumberOfUnits = _context.Units.Where(u => u.PropertyId == p.Id).Count(),
                    ActiveCoverPic = p.ActiveCoverPic
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CountAsync(DateTime asOfDate)
        {
            return await _context.Properties.Where(p => p.CreatedDate <= asOfDate).CountAsync();
        }
        
        public async Task<bool> UpdateRequestedCoverPicAsync(int id, int mediaFileId)
        {
            int rowsAffected = await _context.Database.ExecuteSqlAsync($"UPDATE [Properties] SET RequestedCoverPicId = {mediaFileId}, ModifiedDate = {DateTime.Now} WHERE Id = {id}");
            return rowsAffected > 0;
        }

        public async Task<bool> CommitActiveCoverPicAsync(int id, int mediaFileId)
        {
            int rowsAffected = await _context.Database.ExecuteSqlAsync($"UPDATE [Properties] SET ActiveCoverPicId = {mediaFileId}, ModifiedDate = {DateTime.Now} WHERE Id = {id} AND RequestedCoverPicId = {mediaFileId}");

            return rowsAffected > 0;
        }
    }
}
