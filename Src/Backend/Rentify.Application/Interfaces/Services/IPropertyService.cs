using Rentify.Application.DTOs;
using Rentify.Application.DTOs.Location;
using Rentify.Application.DTOs.MediaFile;
using Rentify.Application.DTOs.Property;

namespace Rentify.Application.Interfaces.Services;

public interface IPropertyService
{
    Task<PaginatedList<PropertySummaryDto>> GetAllPropertyAsync(PropertySearchDto propertySearchDto);
    Task<PropertyDto> GetPropertyByIdAsync(int id);
    Task<PropertyDetailsDto> UpdatePropertyAsync(int id, UpdatePropertyDetailsDto updatePropertyDto);
    Task<PropertyDetailsDto> DeletePropertyAsync(int id);
    Task<PropertyLocationDto> UpdatePropertyLocationAsync(int id, UpdatePropertyLocationDto updatePropertyLocationDto);
    Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createPropertyDto);
    Task<MediaFileDto> UpdatePropertyCoverPicAsync(UpdatePropertyCoverRequestDto updateCoverDto);
    Task<MediaFileDto> DeleteMediaFileAsync(int mediaFileId);
}
