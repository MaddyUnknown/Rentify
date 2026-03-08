using Rentify.Core.Constants;
using Rentify.Core.Entities;
using Rentify.Core.Enums;
using Rentify.Core.Events;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.Core.UnitOfWork;
using Rentify.Event.Application.Constants;
using Rentify.Event.Application.Contexts;
using Rentify.Event.Application.Interfaces;
using Rentify.Event.Application.Utils;
using Rentify.Event.Core;
using Rentify.Event.Core.Contexts;
using Rentify.FileWorkflow.Core.Generators;
using Rentify.Storage.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Application.Handlers
{
    public class SetNewPropertyCoverPicHandler : IMessageHandler<SetNewPropertyCoverPicEvent>
    {
        private IUnitOfWork _unitOfWork;
        private IMediaFileRepository _mediaFileRepository;
        private IPropertyRepository _propertyRepository;
        private IMediaProcessorResolver _mediaProcessorResolver;

        public SetNewPropertyCoverPicHandler(IUnitOfWork unitOfWork, IMediaFileRepository mediaFileRepository, IPropertyRepository propertyRepository, IMediaProcessorResolver mediaProcessorResolver)
        {
            _unitOfWork = unitOfWork;
            _mediaFileRepository = mediaFileRepository;
            _propertyRepository = propertyRepository;
            _mediaProcessorResolver = mediaProcessorResolver;
        }

        public async Task HandleAsync(SetNewPropertyCoverPicEvent message, IMessageProcessingContext context)
        {
            var mediaFile = await _mediaFileRepository.GetMediaFileByIdAsync(message.MediaFileId);
            var mediaFileLink = mediaFile?.MediaFileLink;

            if (mediaFile == null) return; // Skip as the media file is deleted
            if (mediaFileLink == null || mediaFile.MediaFileLink.EntityId != message.PropertyId || mediaFile.MediaFileLink.EntityType != MediaFileEntityEnum.Property) throw new InvalidOperationException(string.Format(MediaFileConstants.MediaFileLinkMissmatchError, message.MediaFileId, message.PropertyId, MediaFileEntityEnum.Property));
            if (mediaFile.Status != MediaFileStatusEnum.Processed) throw new InvalidOperationException(string.Format(MediaFileConstants.MediaFileProcessingForStatusError, mediaFile.Id, mediaFile.Status));

            // Generate cover picture
            var processorContext = new MediaProcessingContext { ContentType = mediaFile.ContentType, MediaFileEntity = mediaFileLink.EntityType, Variant = MediaFileVariantEnum.CoverPic };
            var processor = _mediaProcessorResolver.Resolve(processorContext);
            await processor.ProcessAsync(_unitOfWork, mediaFile, processorContext);
            await _unitOfWork.SaveChangesAsync();

            //Commit media file as cover pic, this update takes into account if the mediaFile was first marked as requested or not, if not then update is skipped
            await _propertyRepository.CommitActiveCoverPicAsync(message.PropertyId, message.MediaFileId);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
