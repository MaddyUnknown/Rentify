using Rentify.Core.Entities;
using Rentify.Core.Enums;
using Rentify.Core.Events;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.Core.UnitOfWork;
using Rentify.Event.Application.Constants;
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
    public class SetNewCoverImageHandler : IMessageHandler<SetNewCoverImageEvent>
    {
        private IUnitOfWork _unitOfWork;
        private IMediaFileRepository _mediaFileRepository;
        private IRepository<MediaFileVariant> _mediaFileVariantCRUDRepository;
        private IMediaFileLinkRepository _mediaFileLinkRepository;
        private IImageThumbnailGenerator _thumbnailGenerator;
        private IFileStorageService _fileStorage;
        private (int width, int height) _coverImageDimension;

        public SetNewCoverImageHandler(IUnitOfWork unitOfWork, IMediaFileRepository mediaFileRepository, IRepository<MediaFileVariant> mediaFileVariantCRUDRepository, IMediaFileLinkRepository mediaFileLinkRepository, IImageThumbnailGenerator imageThumbnailGenerator, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mediaFileRepository = mediaFileRepository;
            _mediaFileVariantCRUDRepository = mediaFileVariantCRUDRepository;
            _mediaFileLinkRepository = mediaFileLinkRepository;
            _thumbnailGenerator = imageThumbnailGenerator;
            _fileStorage = fileStorageService;
            _coverImageDimension = (540, 320); //TO-DO: Replace in config
        }

        public async Task HandleAsync(SetNewCoverImageEvent message, IMessageProcessingContext context)
        {
            var mediaFile = await _mediaFileRepository.GetMediaFileByIdAsync(message.MediaFileId);
            var mediaFileLink = mediaFile?.MediaFileLink;

            if (mediaFile == null) return; // Skip as the media file is deleted
            if (mediaFileLink == null || mediaFile.MediaFileLink.Id != message.MediaFileLinkId) throw new InvalidOperationException(string.Format(MediaFileConstants.MediaFileLinkMissmatchError, message.MediaFileId, message.MediaFileLinkId));
            if (mediaFile.Status != MediaFileStatusEnum.Processed) throw new InvalidOperationException(string.Format(MediaFileConstants.MediaFileProcessingForStatusError, mediaFile.Id, mediaFile.Status));

            //Cover Image generation and save
            if (mediaFile.MediaFileVariants?.Any(m => m.VariantType == MediaFileVariantEnum.Cover) == false)
            {
                using var imageStream = await _fileStorage.ReadAsync(mediaFile.FileKey);

                var coverImageKey = StorageKeyHelper.GenerateFileKeyForMediaFileVariant(mediaFile.FileKey, MediaFileVariantEnum.Cover);
                using var coverImageStream = _thumbnailGenerator.Generate(imageStream, _coverImageDimension.width, _coverImageDimension.height);

                await _fileStorage.DeleteAsync(coverImageKey); //Delete if any
                await _fileStorage.WriteAsync(coverImageKey, coverImageStream);

                //Save cover to DB
                var coverImageVariant = new MediaFileVariant
                {
                    VariantType = MediaFileVariantEnum.Cover,
                    FileKey = coverImageKey,
                    ContentType = MediaFileConstants.ContentType.ImageJpg,
                    MediaFileId = message.MediaFileId,
                    Status = MediaFileVariantStatusEnum.Processed
                };

                _mediaFileVariantCRUDRepository.Add(coverImageVariant);
                await _unitOfWork.SaveChangesAsync();
            }

            //Commit media file as cover pic, this update takes into account if the mediaFile was first marked as requested or not, if not then update is skipped
            await _mediaFileLinkRepository.CommitRequestedCoverForEntityAsync(mediaFileLink.Id, mediaFileLink.EntityType, mediaFileLink.EntityId);
        }
    }
}
