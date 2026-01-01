using Rentify.Application.Constants;
using Rentify.Application.DTOs;
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
        public static NewFileKeyDto GenerateNewFileKeyForMediaFile(MediaFileEntityEnum entityType, int entityId)
        {
            var guid = Guid.NewGuid();
            return new NewFileKeyDto
            {
                TempFileKey = string.Format(MediaFileConstants.MediaTempFileKeyTemplate, entityType, entityId, guid),
                FileKey = string.Format(MediaFileConstants.MediaFileKeyTemplate, entityType, entityId, guid)
            };
        }
    }
}
