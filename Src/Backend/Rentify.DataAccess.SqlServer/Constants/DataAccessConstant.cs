using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.SqlServer.Constants
{
    public static class DataAccessConstant
    {
        public static readonly string ConnectionStringNotConfigurated = "Connection string not configured for data access";
        public static readonly string TransactionInProgressStarted = "A transaction is already in progress.";
        public static readonly string TransactionNotActive = "No active transaction found.";

    }
}
