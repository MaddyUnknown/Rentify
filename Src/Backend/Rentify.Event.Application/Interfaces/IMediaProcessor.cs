using Rentify.Core.Entities;
using Rentify.Core.Enums;
using Rentify.Core.Events;
using Rentify.DataAccess.Core.UnitOfWork;
using Rentify.Event.Application.Contexts;
using Rentify.FileWorkflow.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Application.Interfaces
{
    public interface IMediaProcessor
    {
        bool CanProcess(MediaProcessingContext mediaFileMessage);

        Task ProcessAsync(IUnitOfWork unitOfWork, MediaFile mediaFile, MediaProcessingContext mediaProcessingContext);
    }
}
