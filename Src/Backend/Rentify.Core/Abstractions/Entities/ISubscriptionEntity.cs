using Rentify.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Core.Abstractions.Entities
{
    public interface ISubscriptionEntity
    {
        int SubscriptionId { get; }
        Subscription Subscription { get; }
    }
}
