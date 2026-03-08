using Microsoft.Extensions.Logging;
using Rentify.Core.Enums;
using Rentify.Event.Application.Constants;
using Rentify.Event.Application.Contexts;
using Rentify.Event.Application.Handlers;
using Rentify.Event.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Application.Resolvers
{
    public class MediaProcessorResolver : IMediaProcessorResolver
    {
        private IReadOnlyList<IMediaProcessor> _processors;

        public MediaProcessorResolver(IEnumerable<IMediaProcessor> processors)
        {
            _processors = processors.ToList();
        }

        public IMediaProcessor Resolve(MediaProcessingContext context)
        {
            var processor = _processors.FirstOrDefault(p => p.CanProcess(context));
            if (processor == null) throw new NotSupportedException(string.Format(MediaFileConstants.MediaFileProcessorNotFound, context.MediaFileEntity, context.ContentType, context.Variant));

            return processor;
        }
    }
}
