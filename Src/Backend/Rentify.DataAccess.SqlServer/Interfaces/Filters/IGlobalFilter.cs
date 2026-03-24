using Microsoft.EntityFrameworkCore;
using Rentify.DataAccess.SqlServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.SqlServer.Interfaces.Filters
{
    public interface IGlobalFilter
    {
        void OnModelCreating(ModelBuilder modelBuilder, RentifyDbContext context);
    }
}
