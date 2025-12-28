using Rentify.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.Unit
{
    public class UnitDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Size { get; set; }
        public UnitStatus Status { get; set; }

        public static UnitDto Empty => new UnitDto();
    }
}
