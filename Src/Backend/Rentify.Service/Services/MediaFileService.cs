using Rentify.Application.Constants;
using Rentify.Application.DTOs.MediaFile;
using Rentify.Application.Interfaces.Resolvers;
using Rentify.Application.Interfaces.Services;
using Rentify.Application.Mappers;
using Rentify.Application.Utils;
using Rentify.Core.Entities;
using Rentify.Core.Enums;
using Rentify.Core.Exceptions;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.Core.UnitOfWork;
using Rentify.Storage.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Services
{
    public class MediaFileService : IMediaFileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediaFileValidatorResolver _mediaFileValidatorResolver;
        private readonly IRepository<MediaFileLink> _mediaFileLinkCRUDRepo;
        private readonly IRepository<MediaFile> _mediaFileCRUDRepo;
        private readonly IMediaFileRepository _mediaFileRepo;
        private readonly IFileStorageService _fileStorageService;

        public MediaFileService(
        IUnitOfWork unitOfWork,
        IMediaFileValidatorResolver mediaFileValidatorResolver,
        IRepository<MediaFileLink> mediaFileLinkCRUDRepo,
        IRepository<MediaFile> mediaFileCRUDRepo,
        IMediaFileRepository mediaFileRepo,
        IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mediaFileValidatorResolver = mediaFileValidatorResolver;
            _mediaFileLinkCRUDRepo = mediaFileLinkCRUDRepo;
            _mediaFileCRUDRepo = mediaFileCRUDRepo;
            _mediaFileRepo = mediaFileRepo;
            _fileStorageService = fileStorageService;
        }

        public async Task<MediaFileDto> DeleteMediaFileAsync(DeleteMediaFileDto deleteMediaDto, CancellationToken ct)
        {
            //DB operation to set status as deleted (File/Variant is deleted in job asynchronously)
            var mediaFile = await _mediaFileRepo.GetMediaFileByIdAsync(deleteMediaDto.Id);
            if (mediaFile == null) throw new AppValidationException(string.Format(MediaFileConstants.MediaFileNotFound, deleteMediaDto.Id));
            if (mediaFile.MediaFileLink?.EntityType != deleteMediaDto.EntityType || mediaFile.MediaFileLink?.EntityId != deleteMediaDto.EntityId) throw new AppValidationException(string.Format(MediaFileConstants.MediaFileNotFound, deleteMediaDto.Id));

            mediaFile.Status = MediaFileStatusEnum.Deleted;
            await _unitOfWork.SaveChangesAsync();

            return MediaFileMapper.MapToMediaFileDto(mediaFile);
        }

        public async Task<IEnumerable<MediaFileDto>> GetMediaFileStatusAsync(IEnumerable<int> ids)
        {
            var mediaFiles = await _mediaFileRepo.GetMediaFileByIdsAsync(ids);
            return mediaFiles.Select(f => MediaFileMapper.MapToMediaFileDtoForPolling(f)).ToList();
        }

        public async Task<MediaFileDto> UploadMediaFileAsync(UploadMediaFileDto uploadMediaDto, CancellationToken ct)
        {
            var validator = _mediaFileValidatorResolver.Resolve(uploadMediaDto.EntityType);
            var errors = await validator.ValidateEntityAsync(uploadMediaDto.EntityId);

            if(errors.Count() > 0) throw new AppValidationException(errors);

            var fileKey = StorageKeyHelper.GenerateNewFileKeyForMediaFile(uploadMediaDto.EntityType, uploadMediaDto.EntityId);

            // Uploading file to storage
            await _fileStorageService.WriteAsync(fileKey, uploadMediaDto.MediaStream, ct);

            // DB operation for uploading
            var mediaFile = new MediaFile
            {
                Name = uploadMediaDto.FileName,
                ContentType = MediaFileConstants.ContentType.ImagePng,
                FileKey = fileKey,
                Status = MediaFileStatusEnum.Uploaded
            };

            var mediaFileLink = new MediaFileLink
            {
                EntityId = uploadMediaDto.EntityId,
                EntityType = uploadMediaDto.EntityType,
                MediaFile = mediaFile
            };

            _mediaFileCRUDRepo.Add(mediaFile);
            _mediaFileLinkCRUDRepo.Add(mediaFileLink);

            await _unitOfWork.SaveChangesAsync();

            return MediaFileMapper.MapToMediaFileDto(mediaFile);
        }
    }
}
