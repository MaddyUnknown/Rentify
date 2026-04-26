using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.Subscription
{
    public class SubscriptionDto
    {
        public int Id { get; set; }
        public Guid ReferenceId { get; set; }
        public int OwnerUserId { get; set; }
    }
}
