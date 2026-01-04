using Microsoft.Extensions.DependencyInjection;
using Rentify.Core.Events;
using Rentify.Event.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Hangfire.Extensions
{
    public static class HangfireExtension
    {
        public static void AddHangfireProducerServices(this IServiceCollection services)
        {
            // Add producer service
            services.AddTransient<IMessagePublisher, HangfireMessagePublisher>();
        }

        public static void AddHangfireConsumerServices(this IServiceCollection services)
        {
            // Add consumer service
        }
    }
}
