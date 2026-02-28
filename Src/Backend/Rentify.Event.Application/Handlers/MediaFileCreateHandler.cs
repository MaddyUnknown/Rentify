using Microsoft.Extensions.Logging;
using Rentify.Core.Enums;
using Rentify.Core.Entities;
using Rentify.Core.Events;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.Core.UnitOfWork;
using Rentify.Event.Core;
using Rentify.FileWorkflow.Core.Generators;
using Rentify.Storage.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentify.Event.Application.Constants;
using Rentify.Event.Application.Utils;
using Rentify.Event.Core.Contexts;
using Rentify.Core.Constants;
using Rentify.Event.Application.Contexts;
using Rentify.Event.Application.Interfaces;

namespace Rentify.Event.Application.Handlers
{
    public class MediaFileCreateHandler : IMessageHandler<MediaFileCreateEvent>
    {
        private IUnitOfWork _unitOfWork;
        private IMediaFileRepository _mediaFileRepository;
        private IMediaProcessorResolver _mediaProcessorResolver;

        public MediaFileCreateHandler(IUnitOfWork unitOfWork, IMediaFileRepository mediaFileRepository, IMediaProcessorResolver mediaProcessorResolver)
        {
            _unitOfWork = unitOfWork;
            _mediaFileRepository = mediaFileRepository;
            _mediaProcessorResolver = mediaProcessorResolver;
        }

        public async Task HandleAsync(MediaFileCreateEvent message, IMessageProcessingContext context)
        {
            var mediaFile = await _mediaFileRepository.GetMediaFileByIdAsync(message.MediaFileId);

            if (mediaFile == null) throw new InvalidOperationException(string.Format(MediaFileConstants.MediaFileNotFoundForId, message.MediaFileId));
            if (mediaFile.Status == MediaFileStatusEnum.Processed || mediaFile.Status == MediaFileStatusEnum.Failed) return;
            if (mediaFile.Status == MediaFileStatusEnum.Deleted) throw new InvalidOperationException(string.Format(MediaFileConstants.MediaFileProcessingForStatusError, mediaFile.Id, mediaFile.Status));

            try
            {
                //Variant processing
                foreach(var variant in message.Variants)
                {
                    var processingContext = new MediaProcessingContext { ContentType = mediaFile.ContentType, MediaFileEntity = message.MediaFileEntity, Variant = variant };
                    var processor = _mediaProcessorResolver.Resolve(processingContext);

                    await processor.ProcessAsync(_unitOfWork, mediaFile, processingContext);
                }


                mediaFile.Status = MediaFileStatusEnum.Processed;
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                if(context.IsLastRetry)
                {
                    mediaFile.Status = MediaFileStatusEnum.Failed;
                    await _unitOfWork.SaveChangesAsync();
                }
                throw;
            }
        }
    }
}
