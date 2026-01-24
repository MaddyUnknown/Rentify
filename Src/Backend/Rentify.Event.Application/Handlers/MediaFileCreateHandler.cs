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

namespace Rentify.Event.Application.Handlers
{
    public class MediaFileCreateHandler : IMessageHandler<MediaFileCreateEvent>
    {
        private IUnitOfWork _unitOfWork;
        private IMediaFileRepository _mediaFileRepository;
        private IRepository<MediaFileVariant> _mediaFileVariantCRUDRepository;
        private IImageThumbnailGenerator _thumbnailGenerator;
        private IFileStorageService _fileStorage;
        private (int width, int height) _thumbnailDimension;

        public MediaFileCreateHandler(IUnitOfWork unitOfWork, IMediaFileRepository mediaFileRepository, IRepository<MediaFileVariant> mediaFileVariantCRUDRepository, IImageThumbnailGenerator imageThumbnailGenerator, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mediaFileRepository = mediaFileRepository;
            _mediaFileVariantCRUDRepository = mediaFileVariantCRUDRepository;
            _thumbnailGenerator = imageThumbnailGenerator;
            _fileStorage = fileStorageService;
            _thumbnailDimension = (200, 200);
        }

        public async Task HandleAsync(MediaFileCreateEvent message, IMessageProcessingContext context)
        {
            var mediaFile = await _mediaFileRepository.GetMediaFileByIdAsync(message.MediaFileId);

            if (mediaFile == null) throw new InvalidOperationException(string.Format(MediaFileConstants.MediaFileNotFoundForId, message.MediaFileId));
            if (mediaFile.Status == MediaFileStatusEnum.Processed || mediaFile.Status == MediaFileStatusEnum.Failed) return;
            if (mediaFile.Status == MediaFileStatusEnum.Deleted) throw new InvalidOperationException(string.Format(MediaFileConstants.MediaFileProcessingForStatusError, mediaFile.Id, mediaFile.Status));

            try
            {
                //Thumbnail generation
                if (mediaFile.MediaFileVariants?.Any(m => m.VariantType == MediaFileVariantEnum.Thumbnail) == false)
                {
                    using var imageStream = await _fileStorage.ReadAsync(mediaFile.FileKey);

                    var thumbnailKey = StorageKeyHelper.GenerateFileKeyForMediaFileVariant(mediaFile.FileKey, MediaFileVariantEnum.Thumbnail);
                    using var thumbnailStream = _thumbnailGenerator.Generate(imageStream, _thumbnailDimension.width, _thumbnailDimension.height);

                    await _fileStorage.DeleteAsync(thumbnailKey); //Delete if any
                    await _fileStorage.WriteAsync(thumbnailKey, thumbnailStream);

                    //Save to DB
                    var thumbnailVariant = new MediaFileVariant
                    {
                        VariantType = MediaFileVariantEnum.Thumbnail,
                        FileKey = thumbnailKey,
                        ContentType = MediaFileConstants.ContentType.ImageJpg,
                        MediaFileId = message.MediaFileId,
                        Status = MediaFileVariantStatusEnum.Processed
                    };

                    _mediaFileVariantCRUDRepository.Add(thumbnailVariant);
                    await _unitOfWork.SaveChangesAsync();
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
