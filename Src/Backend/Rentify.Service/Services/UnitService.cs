using Rentify.Core.Entities;
using Rentify.DataAccess.UnitOfWork;
using Rentify.Service.DTOs;
using Rentify.Service.Interfaces;

namespace Rentify.Service.Services;

public class UnitService : IUnitService
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<UnitDto>> GetAllUnitsAsync()
    {
        var units = await _unitOfWork.Units.GetAllAsync(nameof(Unit.Property));
        return units.Select(u => new UnitDto
        {
            UnitId = u.UnitId,
            Name = u.Name,
            Description = u.Description,
            PropertyId = u.PropertyId,
            PropertyName = u.Property.Name
        });
    }

    public async Task<IEnumerable<UnitDto>> GetUnitsByPropertyIdAsync(int propertyId)
    {
        var units = await _unitOfWork.Units.GetWhereAsync(u => u.PropertyId == propertyId, nameof(Unit.Property));
        return units.Select(u => new UnitDto
        {
            UnitId = u.UnitId,
            Name = u.Name,
            Description = u.Description,
            PropertyId = u.PropertyId,
            PropertyName = u.Property.Name
        });
    }

    public async Task<UnitDto?> GetUnitByIdAsync(int id)
    {
        var unit = await _unitOfWork.Units.GetFirstOrDefaultAsync(u => u.UnitId == id, nameof(Unit.Property));
        if (unit == null)
            return null;

        return new UnitDto
        {
            UnitId = unit.UnitId,
            Name = unit.Name,
            Description = unit.Description,
            PropertyId = unit.PropertyId,
            PropertyName = unit.Property.Name
        };
    }

    public async Task<UnitDto> CreateUnitAsync(CreateUnitDto createUnitDto)
    {
        // Verify property exists
        var property = await _unitOfWork.Properties.GetByIdAsync(createUnitDto.PropertyId);
        if (property == null)
            throw new ArgumentException("Property not found", nameof(createUnitDto.PropertyId));

        var unit = new Unit
        {
            Name = createUnitDto.Name,
            Description = createUnitDto.Description,
            PropertyId = createUnitDto.PropertyId
        };

        _unitOfWork.Units.Add(unit);
        await _unitOfWork.SaveChangesAsync();

        return new UnitDto
        {
            UnitId = unit.UnitId,
            Name = unit.Name,
            Description = unit.Description,
            PropertyId = unit.PropertyId,
            PropertyName = property.Name
        };
    }

    public async Task<UnitDto?> UpdateUnitAsync(int id, UpdateUnitDto updateUnitDto)
    {
        var unit = await _unitOfWork.Units.GetFirstOrDefaultAsync(u => u.UnitId == id, nameof(Unit.Property));
        if (unit == null)
            return null;

        // Verify property exists if changing property
        var property = unit.Property;

        if (unit.PropertyId != updateUnitDto.PropertyId)
        {
            property = await _unitOfWork.Properties.GetByIdAsync(updateUnitDto.PropertyId);
            if (property == null)
                throw new ArgumentException("Property not found", nameof(updateUnitDto.PropertyId));
        }

        unit.Name = updateUnitDto.Name;
        unit.Description = updateUnitDto.Description;
        unit.Property = property;

        await _unitOfWork.SaveChangesAsync();

        return new UnitDto
        {
            UnitId = unit.UnitId,
            Name = unit.Name,
            Description = unit.Description,
            PropertyId = unit.PropertyId,
            PropertyName = property.Name
        };
    }

    public async Task<bool> DeleteUnitAsync(int id)
    {
        var unit = await _unitOfWork.Units.GetByIdAsync(id);
        if (unit == null)
            return false;

        _unitOfWork.Units.Remove(unit);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
