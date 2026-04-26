using Rentify.Application.DTOs.Subscription;
using Rentify.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Mappers
{
    public class SubscriptionMapper
    {
        public static SubscriptionDto ToSubscriptionDto(Subscription subscription)
        {
            return new SubscriptionDto
            {
                Id = subscription.Id,
                ReferenceId = subscription.ReferenceId,
                OwnerUserId = subscription.OwnerUserId
            };
        }
    }
}
