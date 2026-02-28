using Microsoft.Extensions.DependencyInjection;
using Rentify.Core.Events;
using Rentify.Event.Application.Handlers;
using Rentify.Event.Application.Interfaces;
using Rentify.Event.Application.Processors;
using Rentify.Event.Application.Resolvers;
using Rentify.Event.Application.Services;
using Rentify.Event.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Application.Extensions
{
    public static class ApplicationEventExtensions
    {
        public static void AddDispatcherServices(this IServiceCollection serviceCollection)
        {
            // Add Service
            serviceCollection.AddTransient<IEventOutboxService, EventOutboxService>();
        }

        public static void AddMessageHandlerServices(this IServiceCollection serviceCollection)
        {
            // Add Event handlers
            serviceCollection.AddTransient<IMessageHandler<MediaFileCreateEvent>, MediaFileCreateHandler>();
            serviceCollection.AddTransient<IMessageHandler<MediaFileDeleteEvent>, MediaFileDeleteHandler>();
            serviceCollection.AddTransient<IMessageHandler<SetNewCoverImageEvent>, SetNewCoverImageHandler>();

            // Add Media File processor resolver
            serviceCollection.AddTransient<IMediaProcessorResolver, MediaProcessorResolver>();

            // Add Media File processor
            serviceCollection.AddTransient<IMediaProcessor, ImageCoverProcessor>();
            serviceCollection.AddTransient<IMediaProcessor, ImageThumbnailProcessor>();
        }
    }
}
