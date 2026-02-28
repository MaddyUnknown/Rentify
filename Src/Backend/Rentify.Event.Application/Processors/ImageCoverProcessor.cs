using Rentify.Core.Constants;
using Rentify.Core.Entities;
using Rentify.Core.Enums;
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
    public class ImageCoverProcessor : IMediaProcessor
    {
        private IRepository<MediaFileVariant> _mediaFileVariantCRUDRepository;
        private IImageThumbnailGenerator _thumbnailGenerator;
        private IFileStorageService _fileStorage;
        private (int width, int height) _coverImageDimension;
        public ImageCoverProcessor(IRepository<MediaFileVariant> mediaFileVariantCRUDRepository, IImageThumbnailGenerator imageThumbnailGenerator, IFileStorageService fileStorageService)
        {
            _mediaFileVariantCRUDRepository = mediaFileVariantCRUDRepository;
            _thumbnailGenerator = imageThumbnailGenerator;
            _fileStorage = fileStorageService;
            _coverImageDimension = (540, 320); //TO-DO: Replace in config
        }

        public bool CanProcess(MediaProcessingContext mediaFileMessage)
        {
            return mediaFileMessage.Variant == MediaFileVariantEnum.CoverPic
                && (
                    mediaFileMessage.ContentType == MediaContentType.ImageJpg
                    || mediaFileMessage.ContentType == MediaContentType.ImagePng
                );
        }

        public async Task ProcessAsync(IUnitOfWork unitOfWork, MediaFile mediaFile, MediaProcessingContext mediaProcessingContext)
        {
            //Cover Image generation and save
            if (mediaFile.MediaFileVariants?.Any(m => m.VariantType == MediaFileVariantEnum.CoverPic) == false)
            {
                using var imageStream = await _fileStorage.ReadAsync(mediaFile.FileKey);

                var coverImageKey = StorageKeyHelper.GenerateFileKeyForMediaFileVariant(mediaFile.FileKey, MediaFileVariantEnum.CoverPic);
                using var coverImageStream = _thumbnailGenerator.Generate(imageStream, _coverImageDimension.width, _coverImageDimension.height);

                await _fileStorage.DeleteAsync(coverImageKey); //Delete if any
                await _fileStorage.WriteAsync(coverImageKey, coverImageStream);

                //Save cover to DB
                var coverImageVariant = new MediaFileVariant
                {
                    VariantType = MediaFileVariantEnum.CoverPic,
                    FileKey = coverImageKey,
                    ContentType = MediaContentType.ImageJpg,
                    MediaFileId = mediaFile.Id,
                    Status = MediaFileVariantStatusEnum.Processed
                };

                _mediaFileVariantCRUDRepository.Add(coverImageVariant);
                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}
