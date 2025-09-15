using Rentify.Core.Entities;
using Rentify.DataAccess.UnitOfWork;
using Rentify.Service.DTOs;
using Rentify.Service.Interfaces;

namespace Rentify.Service.Services;

public class PropertyService : IPropertyService
{
    private readonly IUnitOfWork _unitOfWork;

    public PropertyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync()
    {
        var properties = await _unitOfWork.Properties.GetAllAsync();
        return properties.Select(p => new PropertyDto
        {
            PropertyId = p.PropertyId,
            Name = p.Name,
            Address = p.Address,
            Description = p.Description
        });
    }

    public async Task<PropertyDto?> GetPropertyByIdAsync(int id)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(id);
        if (property == null)
            return null;

        return new PropertyDto
        {
            PropertyId = property.PropertyId,
            Name = property.Name,
            Address = property.Address,
            Description = property.Description
        };
    }

    public async Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createPropertyDto)
    {
        var property = new Property
        {
            Name = createPropertyDto.Name,
            Address = createPropertyDto.Address,
            Description = createPropertyDto.Description
        };

        _unitOfWork.Properties.Add(property);
        await _unitOfWork.SaveChangesAsync();

        return new PropertyDto
        {
            PropertyId = property.PropertyId,
            Name = property.Name,
            Address = property.Address,
            Description = property.Description
        };
    }

    public async Task<PropertyDto?> UpdatePropertyAsync(int id, UpdatePropertyDto updatePropertyDto)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(id);
        if (property == null)
            return null;

        property.Name = updatePropertyDto.Name;
        property.Address = updatePropertyDto.Address;
        property.Description = updatePropertyDto.Description;

        await _unitOfWork.SaveChangesAsync();

        return new PropertyDto
        {
            PropertyId = property.PropertyId,
            Name = property.Name,
            Address = property.Address,
            Description = property.Description
        };
    }

    public async Task<bool> DeletePropertyAsync(int id)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(id);
        if (property == null)
            return false;

        // Check if property has units before deletion
        var hasUnits = await _unitOfWork.Units.AnyAsync(u => u.PropertyId == id);
        if (hasUnits)
            throw new InvalidOperationException("Cannot delete property with existing units. Delete all units first.");

        _unitOfWork.Properties.Remove(property);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
