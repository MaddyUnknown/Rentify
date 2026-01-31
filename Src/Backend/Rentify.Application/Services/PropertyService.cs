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
    private readonly IMediaFileLinkRepository _mediaFileLinkRepo;
    private readonly IRepository<MediaFile> _mediaFileCRUDRepo;
    private readonly IRepository<EventOutbox> _eventOutboxCRUDRepo;

    public PropertyService(
        IUnitOfWork unitOfWork, 
        IRepository<Property> propertyCRUDRepo, 
        IRepository<Unit> unitCRUDRepo,
        IRepository<MediaFileLink> mediaFileLinkCRUDRepo, 
        IRepository<MediaFile> mediaFileCRUDRepo,
        IMediaFileLinkRepository mediaFileLinkRepo,
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
        _mediaFileLinkRepo = mediaFileLinkRepo;
        _mediaFileCRUDRepo = mediaFileCRUDRepo;
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
        var files = await _mediaFileRepo.GetMediaFilesByEntityAsync(MediaFileEntityEnum.Property, id, true);

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

        // Delete property media
        var mediaFiles = await _mediaFileRepo.GetMediaFilesByEntityAsync(MediaFileEntityEnum.Property, id, true);
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

        _propertyCRUDRepo.Remove(property);
        await _unitOfWork.SaveChangesAsync();

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
            foreach(var media in createPropertyDto.Media)
            {
                var mediaFileLink = new MediaFileLink
                {
                    MediaFileId = media.Id,
                    EntityId = propertyEntity.Id,
                    EntityType = MediaFileEntityEnum.Property,
                };

                _mediaFileLinkCRUDRepo.Add(mediaFileLink);

                if(media.MarkAsCover)
                {
                    // Used to insert the row in DB else Media Link Id is not generated
                    await _unitOfWork.SaveChangesAsync();

                    await _mediaFileLinkRepo.UpdateRequestedCoverForEntityAsync(mediaFileLink.Id, mediaFileLink.EntityType, mediaFileLink.EntityId);

                    // Transactional outbox pattern
                    var eventData = new SetNewCoverImageEvent { MediaFileId = media.Id, MediaFileLinkId = mediaFileLink.Id };
                    var eventOutbox = new EventOutbox
                    {
                        EventObjectType = EventTypeHelper.GetEventObjectType<SetNewCoverImageEvent>(),
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
}
