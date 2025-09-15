using Rentify.Core.Entities;
using Rentify.DataAccess.UnitOfWork;
using Rentify.Service.DTOs;
using Rentify.Service.Interfaces;

namespace Rentify.Service.Services;

public class UtilityService : IUtilityService
{
    private readonly IUnitOfWork _unitOfWork;

    public UtilityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<UtilityDto>> GetAllUtilitiesAsync()
    {
        var utilities = await _unitOfWork.Utilities.GetAllAsync(nameof(Utility.Property));
        return utilities.Select(u => new UtilityDto
        {
            UtilityId = u.UtilityId,
            UtilityType = u.UtilityType,
            ProviderName = u.ProviderName,
            UtilityExternalId = u.UtilityExternalId,
            BillingCycle = u.BillingCycle,
            BillingCycleStartDate = u.BillingCycleStartDate,
            PropertyId = u.PropertyId,
            PropertyName = u.Property.Name
        });
    }

    public async Task<IEnumerable<UtilityDto>> GetUtilitiesByPropertyIdAsync(int propertyId)
    {
        var utilities = await _unitOfWork.Utilities.GetWhereAsync(u => u.PropertyId == propertyId, nameof(Utility.Property));
        return utilities.Select(u => new UtilityDto
        {
            UtilityId = u.UtilityId,
            UtilityType = u.UtilityType,
            ProviderName = u.ProviderName,
            UtilityExternalId = u.UtilityExternalId,
            BillingCycle = u.BillingCycle,
            BillingCycleStartDate = u.BillingCycleStartDate,
            PropertyId = u.PropertyId,
            PropertyName = u.Property.Name
        });
    }

    public async Task<UtilityDto?> GetUtilityByIdAsync(int id)
    {
        var utility = await _unitOfWork.Utilities.GetFirstOrDefaultAsync(u => u.PropertyId == id, nameof(Utility.Property));
        if (utility == null)
            return null;

        return new UtilityDto
        {
            UtilityId = utility.UtilityId,
            UtilityType = utility.UtilityType,
            ProviderName = utility.ProviderName,
            UtilityExternalId = utility.UtilityExternalId,
            BillingCycle = utility.BillingCycle,
            BillingCycleStartDate = utility.BillingCycleStartDate,
            PropertyId = utility.PropertyId,
            PropertyName = utility.Property.Name
        };
    }

    public async Task<UtilityDto> CreateUtilityAsync(CreateUtilityDto createUtilityDto)
    {
        // Verify property exists
        var property = await _unitOfWork.Properties.GetByIdAsync(createUtilityDto.PropertyId);
        if (property == null)
            throw new ArgumentException("Property not found", nameof(createUtilityDto.PropertyId));

        var utility = new Utility
        {
            UtilityType = createUtilityDto.UtilityType,
            ProviderName = createUtilityDto.ProviderName,
            UtilityExternalId = createUtilityDto.UtilityExternalId,
            BillingCycle = createUtilityDto.BillingCycle,
            BillingCycleStartDate = createUtilityDto.BillingCycleStartDate,
            PropertyId = createUtilityDto.PropertyId
        };

        _unitOfWork.Utilities.Add(utility);
        await _unitOfWork.SaveChangesAsync();

        return new UtilityDto
        {
            UtilityId = utility.UtilityId,
            UtilityType = utility.UtilityType,
            ProviderName = utility.ProviderName,
            UtilityExternalId = utility.UtilityExternalId,
            BillingCycle = utility.BillingCycle,
            BillingCycleStartDate = utility.BillingCycleStartDate,
            PropertyId = utility.PropertyId,
            PropertyName = property.Name
        };
    }

    public async Task<UtilityDto?> UpdateUtilityAsync(int id, UpdateUtilityDto updateUtilityDto)
    {
        var utility = await _unitOfWork.Utilities.GetFirstOrDefaultAsync(u => u.UtilityId == id, nameof(Utility.Property));
        if (utility == null)
            return null;

        var property = utility.Property;

        // Verify property exists if changing property
        if (utility.PropertyId != updateUtilityDto.PropertyId)
        {
            property = await _unitOfWork.Properties.GetByIdAsync(updateUtilityDto.PropertyId);
            if (property == null)
                throw new ArgumentException("Property not found", nameof(updateUtilityDto.PropertyId));
        }

        utility.UtilityType = updateUtilityDto.UtilityType;
        utility.ProviderName = updateUtilityDto.ProviderName;
        utility.UtilityExternalId = updateUtilityDto.UtilityExternalId;
        utility.BillingCycle = updateUtilityDto.BillingCycle;
        utility.BillingCycleStartDate = updateUtilityDto.BillingCycleStartDate;
        utility.Property = property;

        await _unitOfWork.SaveChangesAsync();

        return new UtilityDto
        {
            UtilityId = utility.UtilityId,
            UtilityType = utility.UtilityType,
            ProviderName = utility.ProviderName,
            UtilityExternalId = utility.UtilityExternalId,
            BillingCycle = utility.BillingCycle,
            BillingCycleStartDate = utility.BillingCycleStartDate,
            PropertyId = utility.PropertyId,
            PropertyName = property.Name
        };
    }

    public async Task<bool> DeleteUtilityAsync(int id)
    {
        var utility = await _unitOfWork.Utilities.GetByIdAsync(id);
        if (utility == null)
            return false;

        _unitOfWork.Utilities.Remove(utility);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
