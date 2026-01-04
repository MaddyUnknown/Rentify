using Rentify.Application.DTOs.MediaFile;
using Rentify.Application.DTOs.Location;
using Rentify.Application.DTOs.Property;

namespace Rentify.Application.Interfaces.Services;

public interface IPropertyService
{
    Task<PropertyDto> GetPropertyByIdAsync(int id);
    Task<PropertyDetailsDto> UpdatePropertyAsync(int id, UpdatePropertyDetailsDto updatePropertyDto);
    Task<PropertyDetailsDto> DeletePropertyAsync(int id);
    Task<PropertyLocationDto> UpdatePropertyLocationAsync(int id, UpdatePropertyLocationDto updatePropertyLocationDto);
}
