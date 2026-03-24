using Rentify.Application.DTOs.Subscription;
using Rentify.Application.Interfaces.Services;
using Rentify.Application.Mappers;
using Rentify.DataAccess.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private ISubscriptionRepository _subscriptionRepository;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<SubscriptionDto?> GetByReferenceId(Guid referenceId)
        {
            var subscription = await _subscriptionRepository.GetByReferenceId(referenceId);
            return subscription == null ? null : SubscriptionMapper.ToSubscriptionDto(subscription);
        }
    }
}
