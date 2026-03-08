using Rentify.Application.Constants;
using Rentify.Application.DTOs;
using Rentify.Application.DTOs.Location;
using Rentify.Application.DTOs.MediaFile;
using Rentify.Application.DTOs.Property;
using Rentify.Application.Interfaces.Services;
using Rentify.Application.Mappers;
using Rentify.Application.Utils;
using Rentify.Core.Entities;
using Rentify.Core.Enums;
using Rentify.Core.Events;
using Rentify.Core.Exceptions;
using Rentify.Core.Utils;
using Rentify.Core.ValueObjects;
using Rentify.DataAccess.Core.Options;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.Core.UnitOfWork;
using Rentify.Storage.Core;

namespace Rentify.Application.Services;

public class PropertyService : IPropertyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Property> _propertyCRUDRepo;
    private readonly IRepository<Unit> _unitCRUDRepo;
    private readonly IMediaFileRepository _mediaFileRepo;
    private readonly IPropertyRepository _propertyRepo;
    private readonly IUnitRepository _unitRepo;
    private readonly IRepository<MediaFileLink> _mediaFileLinkCRUDRepo;
    private readonly IRepository<EventOutbox> _eventOutboxCRUDRepo;

    public PropertyService(
        IUnitOfWork unitOfWork, 
        IRepository<Property> propertyCRUDRepo, 
        IRepository<Unit> unitCRUDRepo,
        IRepository<MediaFileLink> mediaFileLinkCRUDRepo,
        IUnitRepository unitRepo, 
        IPropertyRepository propertyRepository,
        IMediaFileRepository mediaFileRepo,
        IRepository<EventOutbox> eventOutboxCRUDRepo)
    {
        _unitOfWork = unitOfWork;
        _propertyCRUDRepo = propertyCRUDRepo;
        _unitCRUDRepo = unitCRUDRepo;
        _mediaFileRepo = mediaFileRepo;
        _propertyRepo = propertyRepository;
        _unitRepo = unitRepo;
        _mediaFileLinkCRUDRepo = mediaFileLinkCRUDRepo;
        _eventOutboxCRUDRepo = eventOutboxCRUDRepo;
    }

    public async Task<PaginatedList<PropertySummaryDto>> GetAllPropertyAsync(PropertySearchDto propertySearchDto)
    {
        var skipItems = (propertySearchDto.CurrentPage - 1) * propertySearchDto.TotalItemPerPage;
        var totalItemCount = await _propertyRepo.CountAsync(propertySearchDto.AsOfDate);
        var totalPage = Math.Max(1, Math.Ceiling(totalItemCount*1.0/propertySearchDto.TotalItemPerPage));

        if(propertySearchDto.CurrentPage > totalPage) throw new AppValidationException(string.Format(PropertyConstants.PropertiesOutOfPageError, totalPage, propertySearchDto.CurrentPage));

        var properties = await _propertyRepo.GetAllPropertySummaryAsync(skipItems, propertySearchDto.TotalItemPerPage, propertySearchDto.AsOfDate);

        return new PaginatedList<PropertySummaryDto>(
            propertySearchDto.CurrentPage, 
            totalItemCount, 
            properties.Select(p => PropertyMapper.MapToPropertySummaryDto(p))
        );
    }

    public async Task<PropertyDto> GetPropertyByIdAsync(int id)
    {
        var property = await _propertyCRUDRepo.GetByIdAsync(id);
        if (property == null) throw new AppValidationException(string.Format(PropertyConstants.PropertyNotFound, id));

        var units = await _unitRepo.GetByPropertyIdAsync(id);
        var files = await _mediaFileRepo.GetMediaFilesByEntityAsync(MediaFileEntityEnum.Property, id, new MediaFileFilterOption
        {
            FilterDeletedRecords = true
        });

        return PropertyMapper.MapToPropertyDto(property, units, files);
    }

    public async Task<PropertyDetailsDto> UpdatePropertyAsync(int id,UpdatePropertyDetailsDto updatePropertyDto)
    {
        var property = await _propertyCRUDRepo.GetByIdAsync(id);
        if (property == null) throw new AppValidationException(string.Format(PropertyConstants.PropertyNotFound, id));

        property.Name = updatePropertyDto.Name;
        property.Description = updatePropertyDto.Description;

        property.PropertyAddress = new Address
        {
            StreetName = updatePropertyDto.StreetName,
            City = updatePropertyDto.City,
            State = updatePropertyDto.State,
            ZipCode = updatePropertyDto.ZipCode
        };

        await _unitOfWork.SaveChangesAsync();

        return PropertyMapper.MapToPropertyDetailsDto(property);
    }

    public async Task<PropertyDetailsDto> DeletePropertyAsync(int id)
    {
        var property = await _propertyCRUDRepo.GetByIdAsync(id);
        if (property == null) throw new AppValidationException(string.Format(PropertyConstants.PropertyNotFound, id));

        // Check if property has units before deletion
        var unitCount = await _unitRepo.CountByPropertyIdAsync(id);
        if (unitCount != 0) throw new AppValidationException(PropertyConstants.PropertyDeleteFailForActiveUnits);

        _propertyCRUDRepo.Remove(property);
        await _unitOfWork.SaveChangesAsync();

        // Delete property media
        var mediaFiles = await _mediaFileRepo.GetMediaFilesByEntityAsync(MediaFileEntityEnum.Property, id, new MediaFileFilterOption
        {
            FilterDeletedRecords = true
        });
        foreach(var mediaFile in mediaFiles)
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

        return PropertyMapper.MapToPropertyDetailsDto(property);
    }

    public async Task<PropertyLocationDto> UpdatePropertyLocationAsync(int id,UpdatePropertyLocationDto updatePropertyLocationDto)
    {
        var property = await _propertyCRUDRepo.GetByIdAsync(id);
        if (property == null) throw new AppValidationException(string.Format(PropertyConstants.PropertyNotFound, id));


        Location? propertyLocation = (updatePropertyLocationDto?.Longitude == null ||  updatePropertyLocationDto.Latitude == null)
            ? null
            : new Location
            {
                Latitude = updatePropertyLocationDto.Latitude.Value,
                Longitude = updatePropertyLocationDto.Longitude.Value,
            };

        property.PropertyLocation = propertyLocation;
        await _unitOfWork.SaveChangesAsync();

        return PropertyMapper.MapToPropertyLocationDto(property);
    }

    public async Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createPropertyDto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Add Property
            var propertyEntity = new Property
            {
                Name = createPropertyDto.Details.Name,
                Description = createPropertyDto.Details.Description,
                PropertyAddress = new Address
                {
                    StreetName = createPropertyDto.Details.StreetName,
                    City = createPropertyDto.Details.City,
                    State = createPropertyDto.Details.State,
                    ZipCode = createPropertyDto.Details.ZipCode,
                },
                PropertyLocation = (createPropertyDto.Location == null) ? null : new Location { Latitude = createPropertyDto.Location.Latitude, Longitude = createPropertyDto.Location.Longitude }
            };

            _propertyCRUDRepo.Add(propertyEntity);
            await _unitOfWork.SaveChangesAsync();


            //Add Units
            foreach(var unit in createPropertyDto.Units)
            {
                var unitEntity = new Unit
                {
                    Name = unit.Name,
                    Type = unit.Type,
                    Size = unit.Size,
                    PropertyId = propertyEntity.Id
                };

                _unitCRUDRepo.Add(unitEntity);
            }

            await _unitOfWork.SaveChangesAsync();


            //Add Media Link
            foreach(var mediaFileId in createPropertyDto.MediaFileIds)
            {
                var mediaFileLink = new MediaFileLink
                {
                    MediaFileId = mediaFileId,
                    EntityId = propertyEntity.Id,
                    EntityType = MediaFileEntityEnum.Property,
                };

                _mediaFileLinkCRUDRepo.Add(mediaFileLink);

                if(mediaFileId == createPropertyDto.RequestedCoverPicId)
                {
                    // Used to insert the row in DB else Media Link Id is not generated
                    await _unitOfWork.SaveChangesAsync();

                    await _propertyRepo.UpdateRequestedCoverPicAsync(propertyEntity.Id, mediaFileId);

                    // Transactional outbox pattern
                    var eventData = new SetNewPropertyCoverPicEvent { MediaFileId = mediaFileId, PropertyId = propertyEntity.Id };
                    var eventOutbox = new EventOutbox
                    {
                        EventObjectType = EventTypeHelper.GetEventObjectType<SetNewPropertyCoverPicEvent>(),
                        EventData = JsonSerializerHelper.Serialize(eventData)
                    };

                    _eventOutboxCRUDRepo.Add(eventOutbox);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return await GetPropertyByIdAsync(propertyEntity.Id);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<MediaFileDto> UpdatePropertyCoverPicAsync(UpdatePropertyCoverRequestDto updateCoverDto)
    {
        var mediaFile = await _mediaFileRepo.GetMediaFileByIdAsync(updateCoverDto.MediaFileId);
        if (mediaFile == null) throw new AppValidationException(string.Format(MediaFileConstants.MediaFileNotFound, updateCoverDto.MediaFileId));
        if (mediaFile.MediaFileLink?.EntityType != MediaFileEntityEnum.Property || mediaFile.MediaFileLink?.EntityId != updateCoverDto.PropertyId) throw new AppValidationException(string.Format(MediaFileConstants.MediaFileNotFound, updateCoverDto.MediaFileId));

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            await _propertyRepo.UpdateRequestedCoverPicAsync(updateCoverDto.PropertyId, updateCoverDto.MediaFileId);

            // Transactional outbox pattern
            var eventData = new SetNewPropertyCoverPicEvent { MediaFileId = mediaFile.Id, PropertyId = updateCoverDto.PropertyId };
            var eventOutbox = new EventOutbox
            {
                EventObjectType = EventTypeHelper.GetEventObjectType<SetNewPropertyCoverPicEvent>(),
                EventData = JsonSerializerHelper.Serialize(eventData)
            };

            _eventOutboxCRUDRepo.Add(eventOutbox);

            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        return MediaFileMapper.MapToMediaFileDto(mediaFile);
    }

    public async Task<MediaFileDto> DeleteMediaFileAsync(int mediaFileId)
    {
        //DB operation to set status as deleted (File/Variant is deleted in job asynchronously)
        var mediaFile = await _mediaFileRepo.GetMediaFileByIdAsync(mediaFileId);
        if (mediaFile == null) throw new AppValidationException(string.Format(MediaFileConstants.MediaFileNotFound, mediaFileId));
        if (mediaFile.MediaFileLink != null && mediaFile.MediaFileLink.EntityType != MediaFileEntityEnum.Property) throw new AppValidationException(string.Format(MediaFileConstants.MediaFileNotFound, mediaFileId));

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var propertyId = mediaFile?.MediaFileLink?.EntityId;
            var property = propertyId.HasValue ? await _propertyCRUDRepo.GetByIdAsync(propertyId.Value) : null;

            if(property != null)
            {
                property.ActiveCoverPicId = (mediaFileId == property.ActiveCoverPicId) ? null : property.ActiveCoverPicId;
                property.RequestedCoverPicId = (mediaFileId == property.RequestedCoverPicId) ? null : property.RequestedCoverPicId;

                await _unitOfWork.SaveChangesAsync();
            }

            // Transactional outbox pattern
            var eventData = new MediaFileDeleteEvent { MediaFileId = mediaFileId };
            var eventOutbox = new EventOutbox
            {
                EventObjectType = EventTypeHelper.GetEventObjectType<MediaFileDeleteEvent>(),
                EventData = JsonSerializerHelper.Serialize(eventData)
            };

            //Save media status and event record in DB (Not null due to guard claus)
            mediaFile!.Status = MediaFileStatusEnum.Deleted;
            _eventOutboxCRUDRepo.Add(eventOutbox);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        return MediaFileMapper.MapToMediaFileDto(mediaFile);
    }
}
