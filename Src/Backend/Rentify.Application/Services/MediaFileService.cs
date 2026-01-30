using Rentify.Application.Constants;
using Rentify.Application.DTOs.MediaFile;
using Rentify.Application.Enums;
using Rentify.Application.Interfaces.Resolvers;
using Rentify.Application.Interfaces.Services;
using Rentify.Application.Mappers;
using Rentify.Application.Utils;
using Rentify.Core.Entities;
using Rentify.Core.Enums;
using Rentify.Core.Events;
using Rentify.Core.Exceptions;
using Rentify.Core.Utils;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.Core.UnitOfWork;
using Rentify.FileWorkflow.Core.Inspectors;
using Rentify.Storage.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
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
        private readonly IMediaFileVariantRepository _mediaFileVariantRepo;
        private readonly IRepository<EventOutbox> _eventOutboxCRUDRepo;
        private readonly IMediaFileRepository _mediaFileRepo;
        private readonly IFileInspector _fileInspector;
        private readonly IFileStorageService _fileStorageService;

        public MediaFileService(
        IUnitOfWork unitOfWork,
        IMediaFileValidatorResolver mediaFileValidatorResolver,
        IRepository<MediaFileLink> mediaFileLinkCRUDRepo,
        IRepository<MediaFile> mediaFileCRUDRepo,
        IMediaFileVariantRepository mediaFileVariantRepo,
        IRepository<EventOutbox> eventOutboxCRUDRepo,
        IMediaFileRepository mediaFileRepo,
        IFileInspector fileInspector,
        IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mediaFileValidatorResolver = mediaFileValidatorResolver;
            _mediaFileLinkCRUDRepo = mediaFileLinkCRUDRepo;
            _mediaFileCRUDRepo = mediaFileCRUDRepo;
            _mediaFileVariantRepo = mediaFileVariantRepo;
            _eventOutboxCRUDRepo = eventOutboxCRUDRepo;
            _mediaFileRepo = mediaFileRepo;
            _fileInspector = fileInspector;
            _fileStorageService = fileStorageService;
        }

        public async Task<MediaFileDto> DeleteMediaFileAsync(DeleteMediaFileDto deleteMediaDto, CancellationToken ct)
        {
            //DB operation to set status as deleted (File/Variant is deleted in job asynchronously)
            var mediaFile = await _mediaFileRepo.GetMediaFileByIdAsync(deleteMediaDto.Id);
            if (mediaFile == null) throw new AppValidationException(string.Format(MediaFileConstants.MediaFileNotFound, deleteMediaDto.Id));
            if(mediaFile.MediaFileLink != null && mediaFile.MediaFileLink.EntityType != deleteMediaDto.EntityType) throw new AppValidationException(string.Format(MediaFileConstants.MediaFileNotFound, deleteMediaDto.Id));
            
            // Transactional outbox pattern
            var eventData = new MediaFileDeleteEvent { MediaFileId = mediaFile.Id };
            var eventOutbox = new EventOutbox
            {
                EventObjectType = EventTypeHelper.GetEventObjectType<MediaFileDeleteEvent>(),
                EventData = JsonSerializerHelper.Serialize(eventData)
            };

            //Save media status and event record in DB
            mediaFile.Status = MediaFileStatusEnum.Deleted;
            _eventOutboxCRUDRepo.Add(eventOutbox);

            await _unitOfWork.SaveChangesAsync();

            return MediaFileMapper.MapToMediaFileDto(mediaFile);
        }

        public async Task<IEnumerable<MediaFileDto>> GetMediaFileStatusAsync(IEnumerable<int> ids)
        {
            var mediaFiles = await _mediaFileRepo.GetMediaFileByIdsAsync(ids);
            return mediaFiles.Select(f => MediaFileMapper.MapToMediaFileDtoForPolling(f)).ToList();
        }

        public async Task<MediaFileStreamDto> GetMediaFileStreamAsync(MediaFileStreamSearchDto mediaFileSearchDto, CancellationToken ct)
        {
            var mediaFile = await _mediaFileRepo.GetMediaFileByIdAsync(mediaFileSearchDto.Id);
            if (mediaFile == null) throw new AppValidationException(string.Format(MediaFileConstants.MediaFileVariantNotFound, mediaFileSearchDto.Id, mediaFileSearchDto.VariantType?.ToString() ?? "Original"));
            if (mediaFile.MediaFileLink != null && mediaFile.MediaFileLink.EntityType != mediaFileSearchDto.EntityType) throw new AppValidationException(string.Format(MediaFileConstants.MediaFileVariantNotFound, mediaFileSearchDto.Id, mediaFileSearchDto.VariantType?.ToString() ?? "Original"));

            if (mediaFileSearchDto.VariantType == null)
            {
                var fileStream = await _fileStorageService.ReadAsync(mediaFile.FileKey, ct);
                return MediaFileMapper.MapToMediaFileStreamDto(mediaFile, fileStream);
            }
            else
            {
                var type = mediaFileSearchDto.VariantType switch
                {
                    MediaFileVariantDTOEnum.Thumbnail => MediaFileVariantEnum.Thumbnail,
                    MediaFileVariantDTOEnum.Cover => MediaFileVariantEnum.Cover,
                    _ => MediaFileVariantEnum.None
                };

                var mediaFileVariant = await _mediaFileVariantRepo.GetByMediaFileIdAndVariantTypeAsync(mediaFileSearchDto.Id, type);
                if (mediaFileVariant == null) throw new AppValidationException(string.Format(MediaFileConstants.MediaFileVariantNotFound, mediaFileSearchDto.Id, mediaFileSearchDto.VariantType.ToString()));

                var fileStream = await _fileStorageService.ReadAsync(mediaFileVariant.FileKey, ct);
                return MediaFileMapper.MapToMediaFileStreamDto(mediaFile, mediaFileVariant, fileStream);
            }
        }

        public async Task<MediaFileDto> UpdateCoverImageAsync(UpdateCoverImageDto updateCoverDto)
        {
            var mediaFile = await _mediaFileRepo.GetMediaFileByIdAsync(updateCoverDto.MediaFileId);
            if (mediaFile == null) throw new AppValidationException(string.Format(MediaFileConstants.MediaFileNotFound, updateCoverDto.MediaFileId));
            if (mediaFile.MediaFileLink?.EntityType != updateCoverDto.EntityType || mediaFile.MediaFileLink?.EntityId != updateCoverDto.EntityId) throw new AppValidationException(string.Format(MediaFileConstants.MediaFileNotFound, updateCoverDto.MediaFileId));

            // Transactional outbox pattern
            var eventData = new SetNewCoverImageEvent { MediaFileId = mediaFile.Id, MediaFileLinkId = mediaFile.MediaFileLink.Id };
            var eventOutbox = new EventOutbox
            {
                EventObjectType = EventTypeHelper.GetEventObjectType<SetNewCoverImageEvent>(),
                EventData = JsonSerializerHelper.Serialize(eventData)
            };

            _eventOutboxCRUDRepo.Add(eventOutbox);

            await _unitOfWork.SaveChangesAsync();

            return MediaFileMapper.MapToMediaFileDto(mediaFile);
        }

        public async Task<MediaFileDto> UploadMediaFileAsync(UploadMediaFileDto uploadMediaDto, CancellationToken ct)
        {
            //TO-DO: Add max file length check - To be done with other request validation
            var result = _fileInspector.Inspect(uploadMediaDto.MediaStream);
            if (result.MimeType == null) throw new AppValidationException(MediaFileConstants.MediaTypeNotResolved);

            var validator = _mediaFileValidatorResolver.Resolve(uploadMediaDto.EntityType);
            var errors = await validator.ValidateEntityAsync(uploadMediaDto.FileName, result, uploadMediaDto.EntityId);
            if (errors.Count() > 0) throw new AppValidationException(errors);

            var generatedFileKey = StorageKeyHelper.GenerateNewFileKeyForMediaFile();
            
            // Uploading file to storage
            await _fileStorageService.WriteAsync(generatedFileKey, uploadMediaDto.MediaStream, ct);

            try
            {
                // Start transaction
                await _unitOfWork.BeginTransactionAsync();

                // Save media file to DB
                var mediaFile = new MediaFile
                {
                    Name = uploadMediaDto.FileName,
                    ContentType = result.MimeType,
                    FileKey = generatedFileKey,
                    Status = MediaFileStatusEnum.Uploaded
                };

                _mediaFileCRUDRepo.Add(mediaFile);

                if (uploadMediaDto.EntityId.HasValue)
                {
                    var mediaFileLink = new MediaFileLink
                    {
                        EntityId = uploadMediaDto.EntityId.Value,
                        EntityType = uploadMediaDto.EntityType,
                        MediaFile = mediaFile
                    };

                    _mediaFileLinkCRUDRepo.Add(mediaFileLink);
                }

                
                await _unitOfWork.SaveChangesAsync();

                // Save event record in DB - Transactional outbox pattern
                var eventData = new MediaFileCreateEvent { MediaFileId = mediaFile.Id };
                var eventOutbox = new EventOutbox
                {
                    EventObjectType = EventTypeHelper.GetEventObjectType<MediaFileCreateEvent>(),
                    EventData = JsonSerializerHelper.Serialize(eventData)
                };
            
                _eventOutboxCRUDRepo.Add(eventOutbox);
                await _unitOfWork.SaveChangesAsync();

                // Commit changes
                await _unitOfWork.CommitTransactionAsync();

                return MediaFileMapper.MapToMediaFileDto(mediaFile);
            }
            catch
            {
                // Rollback changes
                await _unitOfWork.RollbackTransactionAsync();
                await _fileStorageService.DeleteAsync(generatedFileKey, ct);

                throw;
            }
        }
    }
}
