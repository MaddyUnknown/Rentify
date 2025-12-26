using Rentify.Application.Constants;
using Rentify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Utils
{
    public static class StorageKeyHelper
    {
        public static string GenerateNewFileKeyForMediaFile(MediaFileEntityEnum entityType, int entityId)
        {
            var guid = Guid.NewGuid();
            return string.Format(MediaFileConstants.MediaFileKeyTemplate, entityType, entityId, guid);
        }
    }
}
