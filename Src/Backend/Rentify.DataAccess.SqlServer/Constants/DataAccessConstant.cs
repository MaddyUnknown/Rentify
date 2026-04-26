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

        public static readonly string SubscriptionIdNull = "Subscription id of null is not accepted for data access mode : {0}";
        public static readonly string MethodNotFound = "Method not found for class name '{0}' and method name '{1}'";
    }
}
