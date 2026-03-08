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
        private Dictionary<MediaFileVariantEnum, (int width, int height)> _variantDimension;

        public ImageThumbnailProcessor(IRepository<MediaFileVariant> mediaFileVariantCRUDRepository, IImageThumbnailGenerator imageThumbnailGenerator, IFileStorageService fileStorageService)
        {
            _fileStorage = fileStorageService;
            _thumbnailGenerator = imageThumbnailGenerator;
            _mediaFileVariantCRUDRepository = mediaFileVariantCRUDRepository;
            _variantDimension = new Dictionary<MediaFileVariantEnum, (int width, int height)>
            {
                { MediaFileVariantEnum.Thumbnail, (200, 200) },
                { MediaFileVariantEnum.CoverPic, (540, 320) },
                { MediaFileVariantEnum.ProfilePic, (100, 100) }
            };
        }

        public bool CanProcess(MediaProcessingContext mediaFileMessage)
        {
            return (
                    mediaFileMessage.Variant == MediaFileVariantEnum.Thumbnail
                    || mediaFileMessage.Variant == MediaFileVariantEnum.CoverPic
                    || mediaFileMessage.Variant == MediaFileVariantEnum.ProfilePic
                )
                && (
                    mediaFileMessage.ContentType == MediaContentType.ImageJpg
                    || mediaFileMessage.ContentType == MediaContentType.ImagePng
                );
        }

        public async Task ProcessAsync(IUnitOfWork unitOfWork, MediaFile mediaFile, MediaProcessingContext mediaProcessingContext)
        {
            if (!_variantDimension.ContainsKey(mediaProcessingContext.Variant)) throw new NotSupportedException(string.Format(MediaFileConstants.MediaFileVariantNotSupported, mediaProcessingContext.Variant.ToString()));

            var dimension = _variantDimension[mediaProcessingContext.Variant];

            //Thumbnail generation
            if (mediaFile.MediaFileVariants?.Any(m => m.VariantType == mediaProcessingContext.Variant) == false)
            {
                using var imageStream = await _fileStorage.ReadAsync(mediaFile.FileKey);

                var thumbnailKey = StorageKeyHelper.GenerateFileKeyForMediaFileVariant(mediaFile.FileKey, mediaProcessingContext.Variant);
                using var thumbnailStream = _thumbnailGenerator.Generate(imageStream, dimension.width, dimension.height);

                await _fileStorage.DeleteAsync(thumbnailKey); //Delete if any
                await _fileStorage.WriteAsync(thumbnailKey, thumbnailStream);

                //Save to DB
                var thumbnailVariant = new MediaFileVariant
                {
                    VariantType = mediaProcessingContext.Variant,
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
