using Rentify.Application.Constants;
using Rentify.Application.DTOs;
using Rentify.Application.DTOs.Unit;
using Rentify.Application.Interfaces.Services;
using Rentify.Application.Mappers;
using Rentify.Core.Entities;
using Rentify.Core.Exceptions;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.Core.UnitOfWork;

namespace Rentify.Application.Services;

public class UnitService : IUnitService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Property> _propertyCRUDRepo;
    private readonly IRepository<Unit> _unitCRUDRepo;

    public UnitService(IUnitOfWork unitOfWork, IRepository<Property> propertyCRUDRepo, IRepository<Unit> unitCRUDRepo)
    {
        _unitOfWork = unitOfWork;
        _propertyCRUDRepo = propertyCRUDRepo;
        _unitCRUDRepo = unitCRUDRepo;
    }

    public async Task<UnitDto> CreateUnitAsync(int propertyId, CreateUnitDto createUnitDto)
    {
        // Verify property exists
        var property = await _propertyCRUDRepo.GetByIdAsync(propertyId);
        if (property == null) throw new AppValidationException(string.Format(PropertyConstants.PropertyNotFound, propertyId));

        var unit = new Unit
        {
            Name = createUnitDto.Name,
            Type = createUnitDto.Type,
            Size = createUnitDto.Size,
            PropertyId = propertyId,
        };

        _unitCRUDRepo.Add(unit);
        await _unitOfWork.SaveChangesAsync();

        return UnitMapper.MapToUnitDto(unit);
    }

    public async Task<UnitDto> UpdateUnitAsync(int id, int propertyId, UpdateUnitDto updateUnitDto)
    {
        var unit = await _unitCRUDRepo.GetByIdAsync(id);
        if (unit == null || propertyId != unit.PropertyId) throw new AppValidationException(string.Format(UnitConstants.UnitNotFound, id));

        unit.Name = updateUnitDto.Name;
        unit.Type = updateUnitDto.Type;
        unit.Size = updateUnitDto.Size;

        await _unitOfWork.SaveChangesAsync();

        return UnitMapper.MapToUnitDto(unit);
    }

    public async Task<UnitDto> DeleteUnitAsync(int id, int propertyId)
    {
        var unit = await _unitCRUDRepo.GetByIdAsync(id);
        if (unit == null || propertyId != unit.PropertyId) throw new AppValidationException(string.Format(UnitConstants.UnitNotFound, id));

        _unitCRUDRepo.Remove(unit);
        await _unitOfWork.SaveChangesAsync();
        return UnitMapper.MapToUnitDto(unit);
    }
}
