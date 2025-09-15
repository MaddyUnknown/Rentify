namespace Rentify.Service.DTOs;

public class UnitDto
{
    public int UnitId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
}

public class CreateUnitDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PropertyId { get; set; }
}

public class UpdateUnitDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PropertyId { get; set; }
}
