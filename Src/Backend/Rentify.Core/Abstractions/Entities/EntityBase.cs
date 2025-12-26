using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Core.Abstractions.Entities
{
    public abstract class EntityBase
    {
        [Required]
        public int Id { get; private set; }

        [Required]
        public DateTime CreatedDate { get; private set; }

        public DateTime? ModifiedDate { get; private set; }
    }
}
