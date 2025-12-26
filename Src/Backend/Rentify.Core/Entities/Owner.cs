using Rentify.Core.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;

namespace Rentify.Core.Entities;

public class Owner : EntityBase
{
    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;
}
