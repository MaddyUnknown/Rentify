using Rentify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Core.Abstractions.Accessors
{
    /// <summary>
    /// DataScopeContext - Used as cross cutting data to make subscription avaliable for all layers in the application
    /// </summary>
    public class DataScope
    {
        public DataScopeModeEnum DataScopeMode { get; private set; }
        public int? SubscriptionId { get; private set; }

        public DataScope(DataScopeModeEnum dataScopeMode, int? subscriptionId = null)
        {
            DataScopeMode = dataScopeMode;
            SubscriptionId = subscriptionId;
        }
    }
}
