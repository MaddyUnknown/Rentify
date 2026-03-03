using Rentify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.Core.Options
{
    public class MediaFileFilterOption
    {
        public bool FilterDeletedRecords { get; set; } = false;
        public IEnumerable<MediaFileLinkTagEnum> TagsAttached { get; set; } = Enumerable.Empty<MediaFileLinkTagEnum>();

        public static MediaFileFilterOption Empty => new MediaFileFilterOption();
    }
}
