using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Constants
{
    public static class MediaFileConstants
    {
        public static readonly string MediaFileNotFound = "Media not found for id '{0}'";
        public static readonly string MediaNotSupported = "Media not supported for entity type '{0}'";
        public static readonly string MediaFileKeyTemplate = @"{0}/{1}/{2}/original";
        public static readonly string MediaTempFileKeyTemplate = @"{0}/{1}/{2}/original.temp";
        public static readonly string InvalidMediaTypeProvided = "Invalid media files provided. Accepted media file type are: {0}";

        public static readonly string MediaTypeNotResolved = "Unable to resolve media type";
        public static readonly string MediaTypeNotMatchingExtension = "Media type '{0}' not matching file extension '{1}'. Accepted media type extensions: {2}";


        public static class ContentType
        {
            public static readonly string ImagePng = "image/png";
            public static readonly string ImageJpg = "image/jpeg";

        }
    }
}
