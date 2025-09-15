using Rentify.Service.DTOs;

namespace Rentify.Service.Interfaces;

public interface IPropertyService
{
    Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync();
    Task<PropertyDto?> GetPropertyByIdAsync(int id);
    Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createPropertyDto);
    Task<PropertyDto?> UpdatePropertyAsync(int id, UpdatePropertyDto updatePropertyDto);
    Task<bool> DeletePropertyAsync(int id);
}
