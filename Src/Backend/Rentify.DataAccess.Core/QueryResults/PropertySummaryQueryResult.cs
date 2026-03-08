using Rentify.Core.Entities;
using Rentify.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.Core.QueryResults
{
    public class PropertySummaryQueryResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Address Address { get; set; } = Address.Empty;
        public int NumberOfUnits { get; set; }
        public MediaFile? ActiveCoverPic { get; set; }
    }
}
