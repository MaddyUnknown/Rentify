using Rentify.Core.Enums;
using Rentify.Event.Application.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Application.Interfaces
{
    public interface IMediaProcessorResolver
    {
        IMediaProcessor Resolve(MediaProcessingContext context);
    }
}
