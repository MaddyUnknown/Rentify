using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Application.Constants
{
    public static class MediaFileConstants
    {
        public static readonly string MediaFileDeleteForStatusError = "Media file with id '{0}' cannot be deleted as current status is '{1}'";
        public static readonly string MediaFileProcessingForStatusError = "Media file with id '{0}' cannot be processed as current status is '{1}'";

        public static readonly string MediaFileLinkMissmatchError = "Media file with id '{0}' is not linked to media file link '1'";

        public static readonly string MediaFileNotFoundForId = "Media file not found for id '{0}'";
        public static readonly string MediaFileVariantKeyTemplate = @"{0}.{1}";

        public static class ContentType
        {
            public static readonly string ImagePng = "image/png";
            public static readonly string ImageJpg = "image/jpeg";
        }
    }
}
