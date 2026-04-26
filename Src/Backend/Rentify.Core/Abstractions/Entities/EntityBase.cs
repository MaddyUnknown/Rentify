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
        public int Id { get; protected set; }

        [Required]
        public DateTime CreatedDate { get; protected set; }

        public DateTime? ModifiedDate { get; protected set; }
    }
}
