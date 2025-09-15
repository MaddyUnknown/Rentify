using Rentify.Service.DTOs;

namespace Rentify.Service.Interfaces;

public interface IUnitService
{
    Task<IEnumerable<UnitDto>> GetAllUnitsAsync();
    Task<IEnumerable<UnitDto>> GetUnitsByPropertyIdAsync(int propertyId);
    Task<UnitDto?> GetUnitByIdAsync(int id);
    Task<UnitDto> CreateUnitAsync(CreateUnitDto createUnitDto);
    Task<UnitDto?> UpdateUnitAsync(int id, UpdateUnitDto updateUnitDto);
    Task<bool> DeleteUnitAsync(int id);
}
