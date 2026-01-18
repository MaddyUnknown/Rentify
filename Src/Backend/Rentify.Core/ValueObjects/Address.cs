using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Core.ValueObjects
{
    public class Address
    {
        [Required]
        [MaxLength(50)]
        public string StreetName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string ZipCode { get; set; } = string.Empty;


        public static Address Empty => new Address();
    }
}
