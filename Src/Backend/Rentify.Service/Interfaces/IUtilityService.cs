using Rentify.Service.DTOs;

namespace Rentify.Service.Interfaces;

public interface IUtilityService
{
    Task<IEnumerable<UtilityDto>> GetAllUtilitiesAsync();
    Task<IEnumerable<UtilityDto>> GetUtilitiesByPropertyIdAsync(int propertyId);
    Task<UtilityDto?> GetUtilityByIdAsync(int id);
    Task<UtilityDto> CreateUtilityAsync(CreateUtilityDto createUtilityDto);
    Task<UtilityDto?> UpdateUtilityAsync(int id, UpdateUtilityDto updateUtilityDto);
    Task<bool> DeleteUtilityAsync(int id);
}
