using Rentify.Core.Constants;
using Rentify.Core.Entities;
using Rentify.Core.Enums;
using Rentify.Core.Events;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.Core.UnitOfWork;
using Rentify.Event.Application.Contexts;
using Rentify.Event.Application.Interfaces;
using Rentify.Event.Application.Utils;
using Rentify.FileWorkflow.Core.Generators;
using Rentify.Storage.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Application.Processors
{
    public class ImageThumbnailProcessor : IMediaProcessor
    {
        private IRepository<MediaFileVariant> _mediaFileVariantCRUDRepository;
        private IImageThumbnailGenerator _thumbnailGenerator;
        private IFileStorageService _fileStorage;
        private (int width, int height) _thumbnailDimension;

        public ImageThumbnailProcessor(IRepository<MediaFileVariant> mediaFileVariantCRUDRepository, IImageThumbnailGenerator imageThumbnailGenerator, IFileStorageService fileStorageService)
        {
            _fileStorage = fileStorageService;
            _thumbnailDimension = (200, 200);
            _thumbnailGenerator = imageThumbnailGenerator;
            _mediaFileVariantCRUDRepository = mediaFileVariantCRUDRepository;
        }

        public bool CanProcess(MediaProcessingContext mediaFileMessage)
        {
            return mediaFileMessage.MediaFileEntity == MediaFileEntityEnum.Tenant 
                && mediaFileMessage.Usage == MediaFileUsageEnum.Thumbnail 
                && (
                    mediaFileMessage.ContentType == MediaContentType.ImageJpg
                    || mediaFileMessage.ContentType == MediaContentType.ImagePng
                );
        }

        public async Task ProcessAsync(IUnitOfWork unitOfWork, MediaFile mediaFile, MediaProcessingContext mediaProcessingContext)
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
                    ContentType = MediaContentType.ImageJpg,
                    MediaFileId = mediaFile.Id,
                    Status = MediaFileVariantStatusEnum.Processed
                };

                _mediaFileVariantCRUDRepository.Add(thumbnailVariant);
                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}
