using Rentify.Core.Enums;

namespace Rentify.Service.DTOs;

public class UtilityDto
{
    public int UtilityId { get; set; }
    public UtilityTypeEnum UtilityType { get; set; } = UtilityTypeEnum.None;
    public string ProviderName { get; set; } = string.Empty;
    public string UtilityExternalId { get; set; } = string.Empty;
    public BillingCycleEnum BillingCycle { get; set; } = BillingCycleEnum.None;
    public DateTime BillingCycleStartDate { get; set; }
    public int PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
}

public class CreateUtilityDto
{
    public UtilityTypeEnum UtilityType { get; set; } = UtilityTypeEnum.None;
    public string ProviderName { get; set; } = string.Empty;
    public string UtilityExternalId { get; set; } = string.Empty;
    public BillingCycleEnum BillingCycle { get; set; } = BillingCycleEnum.None;
    public DateTime BillingCycleStartDate { get; set; }
    public int PropertyId { get; set; }
}

public class UpdateUtilityDto
{
    public UtilityTypeEnum UtilityType { get; set; } = UtilityTypeEnum.None;
    public string ProviderName { get; set; } = string.Empty;
    public string UtilityExternalId { get; set; } = string.Empty;
    public BillingCycleEnum BillingCycle { get; set; } = BillingCycleEnum.None;
    public DateTime BillingCycleStartDate { get; set; }
    public int PropertyId { get; set; }
}
