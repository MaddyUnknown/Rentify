using Rentify.Application.DTOs.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Interfaces.Services
{
    public interface ISubscriptionService
    {
        public Task<SubscriptionDto?> GetByReferenceId(Guid referenceId);
    }
}
