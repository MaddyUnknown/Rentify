using Rentify.Application.Constants;
using Rentify.Application.DTOs;
using Rentify.Application.DTOs.Location;
using Rentify.Application.DTOs.MediaFile;
using Rentify.Application.DTOs.Property;
using Rentify.Application.Interfaces.Services;
using Rentify.Application.Mappers;
using Rentify.Core.Entities;
using Rentify.Core.Enums;
using Rentify.Core.Exceptions;
using Rentify.Core.ValueObjects;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.Core.UnitOfWork;
using Rentify.Storage.Core;

namespace Rentify.Application.Services;

public class PropertyService : IPropertyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Property> _propertyCRUDRepo;
    private readonly IMediaFileRepository _mediaFileRepo;
    private readonly IPropertyRepository _propertyRepo;
    private readonly IUnitRepository _unitRepo;

    public PropertyService(
        IUnitOfWork unitOfWork, 
        IRepository<Property> propertyCRUDRepo, 
        IRepository<MediaFileLink> mediaFileLinkCRUDRepo, 
        IRepository<MediaFile> mediaFileCRUDRepo, 
        IUnitRepository unitRepo, 
        IPropertyRepository propertyRepository,
        IMediaFileRepository mediaFileRepo,
        IFileStorageService fileStorageSerice)
    {
        _unitOfWork = unitOfWork;
        _propertyCRUDRepo = propertyCRUDRepo;
        _mediaFileRepo = mediaFileRepo;
        _propertyRepo = propertyRepository;
        _unitRepo = unitRepo;
    }

    public async Task<PaginatedList<PropertySummaryDto>> GetAllPropertyAsync(PropertySearchDto propertySearchDto)
    {
        var skipItems = (propertySearchDto.CurrentPage - 1) * propertySearchDto.TotalItemPerPage;
        var totalItemCount = await _propertyRepo.CountAsync(propertySearchDto.AsOfDate);
        var totalPage = Math.Ceiling(totalItemCount*1.0/propertySearchDto.TotalItemPerPage);

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
}
