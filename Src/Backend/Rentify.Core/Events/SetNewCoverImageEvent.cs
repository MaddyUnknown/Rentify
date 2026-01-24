using Rentify.Core.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Core.Events
{
    public class SetNewCoverImageEvent : EventBase
    {
        public int MediaFileLinkId { get; set; }
        public int MediaFileId { get; set; }
    }
}
