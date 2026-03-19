using Rentify.Application.Constants;
using Rentify.Application.DTOs;
using Rentify.Application.DTOs.MediaFile;
using Rentify.Application.DTOs.Tenant;
using Rentify.Application.Interfaces.Resolvers;
using Rentify.Application.Interfaces.Services;
using Rentify.Application.Mappers;
using Rentify.Application.Utils;
using Rentify.Core.Constants;
using Rentify.Core.Entities;
using Rentify.Core.Enums;
using Rentify.Core.Events;
using Rentify.Core.Exceptions;
using Rentify.Core.Utils;
using Rentify.Core.ValueObjects;
using Rentify.DataAccess.Core.Options;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.Core.UnitOfWork;
using Rentify.FileWorkflow.Core.Inspectors;
using Rentify.Storage.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Services
{
    public class TenantService : ITenantService
    {
        private readonly IEnumerable<string> ACCEPTED_PROFILE_PIC_CONTENTS = [MediaContentType.ImagePng, MediaContentType.ImageJpg];

        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Tenant> _tenantCRUDRepo;
        private readonly IRepository<TenantEmergencyContact> _tenantEmergencyContactCRUDRepo;
        private readonly IMediaFileRepository _mediaFileRepo;
        private readonly ITenantRepository _tenantRepo;
        private readonly ITenantEmergencyContactRepository _tenantEmergencyContactRepo;
        private readonly IRepository<MediaFileLink> _mediaFileLinkCRUDRepo;
        private readonly IRepository<EventOutbox> _eventOutboxCRUDRepo;
        private readonly IFileInspector _fileInspector;
        private readonly IMediaFileValidatorResolver _mediaFileValidatorResolver;
        private readonly IFileStorageManager _fileStorageService;
        private readonly IRepository<MediaFile> _mediaFileCRUDRepo;

        public TenantService(
            IUnitOfWork unitOfWork,
            IRepository<Tenant> tenantCRUDRepo,
            IRepository<TenantEmergencyContact> tenantEmergencyContactCRUDRepo,
            IRepository<MediaFileLink> mediaFileLinkCRUDRepo,
            ITenantEmergencyContactRepository tenantEmergencyContactRepo,
            ITenantRepository tenantRepository,
            IMediaFileRepository mediaFileRepo,
            IRepository<EventOutbox> eventOutboxCRUDRepo,
            IFileInspector fileInspector,
            IMediaFileValidatorResolver mediaFileValidatorResolver,
            IFileStorageManager fileStorageService,
            IRepository<MediaFile> mediaFileCRUDRepo)
        {
            _unitOfWork = unitOfWork;
            _tenantCRUDRepo = tenantCRUDRepo;
            _tenantEmergencyContactCRUDRepo = tenantEmergencyContactCRUDRepo;
            _mediaFileRepo = mediaFileRepo;
            _tenantRepo = tenantRepository;
            _tenantEmergencyContactRepo = tenantEmergencyContactRepo;
            _mediaFileLinkCRUDRepo = mediaFileLinkCRUDRepo;
            _eventOutboxCRUDRepo = eventOutboxCRUDRepo;
            _fileInspector = fileInspector;
            _mediaFileValidatorResolver = mediaFileValidatorResolver;
            _fileStorageService = fileStorageService;
            _fileStorageService = fileStorageService;
            _mediaFileCRUDRepo = mediaFileCRUDRepo;
        }

        public async Task<PaginatedList<TenantSummaryDto>> GetAllTenantAsync(TenantSearchDto tenantSearchDto)
        {
            var skipItems = (tenantSearchDto.CurrentPage - 1) * tenantSearchDto.TotalItemPerPage;
            var totalItemCount = await _tenantRepo.CountAsync(tenantSearchDto.AsOfDate);
            var totalPage = Math.Max(1, Math.Ceiling(totalItemCount * 1.0 / tenantSearchDto.TotalItemPerPage));

            if (tenantSearchDto.CurrentPage > totalPage) throw new AppValidationException(string.Format(TenantConstants.TenantsOutOfPageError, totalPage, tenantSearchDto.CurrentPage));

            var tenants = await _tenantRepo.GetAllTenantSummaryAsync(skipItems, tenantSearchDto.TotalItemPerPage, tenantSearchDto.AsOfDate);

            return new PaginatedList<TenantSummaryDto>(
                tenantSearchDto.CurrentPage,
                totalItemCount,
                tenants.Select(p => TenantMapper.MapToTenantSummaryDto(p))
            );
        }

        public async Task<TenantDto> GetTenantByIdAsync(int id)
        {
            var tenant = await _tenantCRUDRepo.GetByIdAsync(id);
            if (tenant == null) throw new AppValidationException(string.Format(TenantConstants.TenantNotFound, id));

            var tenantEmergencyContacts = await _tenantEmergencyContactRepo.GetAllByTenantId(id);
            var files = await _mediaFileRepo.GetMediaFilesByEntityAsync(MediaFileEntityEnum.Tenant, id, new MediaFileFilterOption
            {
                FilterDeletedRecords = true
            });

            return TenantMapper.MapToTenantDto(tenant, tenantEmergencyContacts, files);
        }

        public async Task<TenantDetailsDto> UpdateTenantAsync(int id, UpdateTenantDetailsDto updateTenantDto)
        {
            var tenant = await _tenantCRUDRepo.GetByIdAsync(id);
            if (tenant == null) throw new AppValidationException(string.Format(TenantConstants.TenantNotFound, id));

            tenant.Name = updateTenantDto.Name;
            tenant.Email = updateTenantDto.Email;
            tenant.PhoneNumber = updateTenantDto.PhoneNumber;
            tenant.Dob = updateTenantDto.Dob.ToDateTime(TimeOnly.MinValue);
            tenant.Employment = updateTenantDto.Employment;
            tenant.Note = updateTenantDto.Note;
            tenant.TenantAddress = new Address
            {
                StreetName = updateTenantDto.StreetName,
                City = updateTenantDto.City,
                State = updateTenantDto.State,
                ZipCode = updateTenantDto.ZipCode
            };

            await _unitOfWork.SaveChangesAsync();

            return TenantMapper.MapToTenantDetailsDto(tenant);
        }

        public async Task<TenantDetailsDto> DeleteTenantAsync(int id)
        {
            var tenant = await _tenantCRUDRepo.GetByIdAsync(id);
            if (tenant == null) throw new AppValidationException(string.Format(TenantConstants.TenantNotFound, id));

            var tenantEmergencyContacts = await _tenantEmergencyContactRepo.GetAllByTenantId(id);
            _tenantEmergencyContactCRUDRepo.RemoveRange(tenantEmergencyContacts);

            // Delete tenant media
            var mediaFiles = await _mediaFileRepo.GetMediaFilesByEntityAsync(MediaFileEntityEnum.Tenant, id, new MediaFileFilterOption
            {
                FilterDeletedRecords = true
            });

            foreach (var mediaFile in mediaFiles)
            {
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
            }

            _tenantCRUDRepo.Remove(tenant);
            await _unitOfWork.SaveChangesAsync();

            return TenantMapper.MapToTenantDetailsDto(tenant);
        }

        public async Task<TenantEmergencyContactDto> UpdateTenantEmergencyContactAsync(int tenantId, int tenantEmergencyContactId, UpdateTenantEmergencyContactDto updateTenantEmergencyContactDto)
        {
            var tenantEmergencyContact = await _tenantEmergencyContactCRUDRepo.GetByIdAsync(tenantEmergencyContactId);
            if (tenantEmergencyContact == null || tenantEmergencyContact.TenantId != tenantId) throw new AppValidationException(string.Format(TenantConstants.TenantEmergencyDetailsNotFound, tenantEmergencyContactId));

            tenantEmergencyContact.Name = updateTenantEmergencyContactDto.Name;
            tenantEmergencyContact.Relationship = updateTenantEmergencyContactDto.Relationship;
            tenantEmergencyContact.Email = updateTenantEmergencyContactDto.Email;
            tenantEmergencyContact.PhoneNumber = updateTenantEmergencyContactDto.PhoneNumber;

            await _unitOfWork.SaveChangesAsync();

            return TenantMapper.MapToTenantEmergencyContactDto(tenantEmergencyContact);
        }

        public async Task<TenantDto> CreateTenantAsync(CreateTenantDto createTenantDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                //Add Tenant
                var tenantEntity = new Tenant
                {
                    Name = createTenantDto.Details.Name,
                    Email = createTenantDto.Details.Email,
                    PhoneNumber = createTenantDto.Details.PhoneNumber,
                    Dob = createTenantDto.Details.Dob.ToDateTime(TimeOnly.MinValue),
                    Employment = createTenantDto.Details.Employment,
                    Note = createTenantDto.Details.Note,
                    TenantAddress = new Address
                    {
                        StreetName = createTenantDto.Details.StreetName,
                        City = createTenantDto.Details.City,
                        State = createTenantDto.Details.State,
                        ZipCode = createTenantDto.Details.ZipCode,
                    },
                    ProfilePicId = createTenantDto.ProfilePicId
                };

                _tenantCRUDRepo.Add(tenantEntity);
                await _unitOfWork.SaveChangesAsync();


                //Add Tenant Emergency Contact
                var tenantEmergencyContact = new TenantEmergencyContact
                {
                    Name = createTenantDto.EmergencyContact.Name,
                    Relationship = createTenantDto.EmergencyContact.Relationship,
                    Email = createTenantDto.EmergencyContact.Email,
                    PhoneNumber = createTenantDto.EmergencyContact.PhoneNumber,
                    TenantId = tenantEntity.Id,
                };

                _tenantEmergencyContactCRUDRepo.Add(tenantEmergencyContact);
                await _unitOfWork.SaveChangesAsync();

                //Add Profile Pic Link
                if (createTenantDto.ProfilePicId.HasValue)
                {
                    var mediaFileLink = new MediaFileLink
                    {
                        MediaFileId = createTenantDto.ProfilePicId.Value,
                        EntityId = tenantEntity.Id,
                        EntityType = MediaFileEntityEnum.Tenant,
                    };

                    _mediaFileLinkCRUDRepo.Add(mediaFileLink);
                }

                //Add Media Link
                foreach (var mediaFileId in createTenantDto.DocumentIds)
                {
                    var mediaFileLink = new MediaFileLink
                    {
                        MediaFileId = mediaFileId,
                        EntityId = tenantEntity.Id,
                        EntityType = MediaFileEntityEnum.Tenant,
                    };

                    _mediaFileLinkCRUDRepo.Add(mediaFileLink);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return await GetTenantByIdAsync(tenantEntity.Id);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<MediaFileDto> UpdateProfilePic(UpdateTenantProfilePicDto updateTenantProfilePic, CancellationToken ct = default)
        {
            //TO-DO: Add max file length check - To be done with other request validation
            var result = _fileInspector.Inspect(updateTenantProfilePic.MediaStream);
            if (result.MimeType == null) throw new AppValidationException(MediaFileConstants.MediaTypeNotResolved);

            var validator = _mediaFileValidatorResolver.Resolve(MediaFileEntityEnum.Tenant);
            var errors = await validator.ValidateEntityAsync(updateTenantProfilePic.FileName, result, updateTenantProfilePic.TenantId, ACCEPTED_PROFILE_PIC_CONTENTS);
            if (errors.Count() > 0) throw new AppValidationException(errors);

            var generatedFileKey = StorageKeyHelper.GenerateNewFileKeyForMediaFile();

            // Uploading file to storage
            await _fileStorageService.WriteAsync(generatedFileKey, updateTenantProfilePic.MediaStream, ct);

            try
            {
                // Start transaction
                await _unitOfWork.BeginTransactionAsync();

                //Save media file to DB
                var mediaFile = new MediaFile
                {
                    Name = updateTenantProfilePic.FileName,
                    ContentType = result.MimeType,
                    Length = updateTenantProfilePic.Length,
                    FileKey = generatedFileKey,
                    Status = MediaFileStatusEnum.Uploaded
                };

                _mediaFileCRUDRepo.Add(mediaFile);
                await _unitOfWork.SaveChangesAsync();


                if (updateTenantProfilePic.TenantId.HasValue)
                {
                    var tenant = await _tenantCRUDRepo.GetByIdAsync(updateTenantProfilePic.TenantId.Value);

                    //Clean-up old profile pic
                    if(tenant != null && tenant.ProfilePicId.HasValue)
                    {
                        var oldProfilePicId = tenant.ProfilePicId.Value;
                        var oldProfilePic = await _mediaFileCRUDRepo.GetByIdAsync(oldProfilePicId);

                        if(oldProfilePic != null)
                        {
                            var deleteEventData = new MediaFileDeleteEvent { MediaFileId = oldProfilePic.Id };
                            var deleteEventOutbox = new EventOutbox
                            {
                                EventObjectType = EventTypeHelper.GetEventObjectType<MediaFileDeleteEvent>(),
                                EventData = JsonSerializerHelper.Serialize(deleteEventData)
                            };

                            //Save media status and event record in DB
                            oldProfilePic.Status = MediaFileStatusEnum.Deleted;
                            _eventOutboxCRUDRepo.Add(deleteEventOutbox);
                        }
                    }

                    //Update new profile pic
                    if(tenant != null)
                    {
                        tenant.ProfilePicId = mediaFile.Id;
                    }

                    // Add media file link
                    var mediaFileLink = new MediaFileLink
                    {
                        EntityId = updateTenantProfilePic.TenantId.Value,
                        EntityType = MediaFileEntityEnum.Tenant,
                        MediaFile = mediaFile,
                    };

                    _mediaFileLinkCRUDRepo.Add(mediaFileLink);

                    await _unitOfWork.SaveChangesAsync();
                }

                // Save event record in DB - Transactional outbox pattern
                var eventData = new MediaFileCreateEvent
                {
                    MediaFileId = mediaFile.Id,
                    MediaFileEntity = MediaFileEntityEnum.Tenant,
                    Variants = [MediaFileVariantEnum.ProfilePic]
                };

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
