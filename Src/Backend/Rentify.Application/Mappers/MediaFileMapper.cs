using Rentify.Application.DTOs.MediaFile;
using Rentify.Core.Entities;
using Rentify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Mappers
{
    public static class MediaFileMapper
    {
        public static MediaFileDto MapToMediaFileDto(MediaFile file)
        {
            var thumbnailDto = file.MediaFileVariants
                .Where(f => f.VariantType == MediaFileVariantEnum.Thumbnail)
                .Select(v => new MediaFileVariantsDto
                {
                    ContentType = v.ContentType,
                    ProcessingStatus = v.Status
                }).FirstOrDefault();

            return new MediaFileDto
            {
                Id = file.Id,
                Name = file.Name,
                ContentType = file.ContentType,
                ProcessingStatus = file.Status,
                Thumbnail = thumbnailDto
            };
        }

        public static MediaFileDto MapToMediaFileDtoForPolling(MediaFile file)
        {
            if(file.Status == MediaFileStatusEnum.Processed || file.Status == MediaFileStatusEnum.Deleted || file.Status == MediaFileStatusEnum.Failed)
            {
                return MapToMediaFileDto(file);
            }
            else
            {
                return new MediaFileDto
                {
                    Id = file.Id,
                    ProcessingStatus = file.Status
                };
            }
        }

        public static MediaFileStreamDto MapToMediaFileStreamDto(MediaFile file, Stream stream)
        {
            return new MediaFileStreamDto
            {
                MediaStream = stream,
                ContentType = file.ContentType,
                FileName = file.Name
            };
        }

        public static MediaFileStreamDto MapToMediaFileStreamDto(MediaFile file, MediaFileVariant fileVariant, Stream stream)
        {
            return new MediaFileStreamDto
            {
                MediaStream = stream,
                ContentType = fileVariant.ContentType,
                FileName = $"{fileVariant.VariantType}_{file.Name}"
            };
        }
    }
}
