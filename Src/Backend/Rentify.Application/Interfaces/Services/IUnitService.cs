using Rentify.Application.DTOs;
using Rentify.Application.DTOs.Unit;

namespace Rentify.Application.Interfaces.Services;

public interface IUnitService
{
    Task<UnitDto> CreateUnitAsync(int propertyId, CreateUnitDto createUnitDto);
    Task<UnitDto> UpdateUnitAsync(int id, int propertyId, UpdateUnitDto updateUnitDto);
    Task<UnitDto> DeleteUnitAsync(int id, int propertyId);
}
