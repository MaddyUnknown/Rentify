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
        public static MediaFileDto MapToMediaFileDto(MediaFile file, bool checkCoverRequested = true)
        {
            var variants = file.MediaFileVariants.ToDictionary(
                v => v.VariantType,
                v => new MediaFileVariantsDto
                {
                    ContentType = v.ContentType,
                    ProcessingStatus = v.Status,
                }
            );

            return new MediaFileDto
            {
                Id = file.Id,
                Name = file.Name,
                Length = file.Length,
                ContentType = file.ContentType,
                UploadedDate = file.CreatedDate,
                ProcessingStatus = file.Status,
                MarkedAsCover = checkCoverRequested ? file?.MediaFileLink?.MarkAsCoverRequested : file?.MediaFileLink?.MarkedAsCover,
                Variants = variants
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
