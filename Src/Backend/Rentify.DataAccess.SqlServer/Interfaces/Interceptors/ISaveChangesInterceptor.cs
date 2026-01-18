using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.SqlServer.Interfaces.Interceptors
{
    public interface ISaveChangesInterceptor
    {
        void OnSaveChange(DbContext dbContext);
    }
}
