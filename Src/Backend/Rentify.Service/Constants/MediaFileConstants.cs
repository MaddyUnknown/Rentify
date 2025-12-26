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

        public static class ContentType
        {
            public static readonly string ImagePng = "image/png";
        }
    }
}
