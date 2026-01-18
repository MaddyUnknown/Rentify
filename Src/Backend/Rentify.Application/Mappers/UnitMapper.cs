using Rentify.Application.DTOs.Property;
using Rentify.Application.DTOs.Unit;
using Rentify.Application.Enums;
using Rentify.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Mappers
{
    public static class UnitMapper
    {
        public static UnitDto MapToUnitDto(Unit unit)
        {
            return new UnitDto
            {
                Id = unit.Id,
                Name = unit.Name,
                Type = unit.Type,
                Size = unit.Size,
                Status = UnitStatus.Vacant
            };
        }
    }
}
