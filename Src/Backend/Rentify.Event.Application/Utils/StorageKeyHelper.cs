using Rentify.Core.Enums;
using Rentify.Event.Application.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Application.Utils
{
    public static class StorageKeyHelper
    {
        public static string GenerateFileKeyForMediaFileVariant(string originalKey, MediaFileVariantEnum variantType)
        {
            return string.Format(MediaFileConstants.MediaFileVariantKeyTemplate, originalKey, variantType);
        }
    }
}
