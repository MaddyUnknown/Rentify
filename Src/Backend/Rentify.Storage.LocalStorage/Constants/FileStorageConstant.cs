using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Storage.LocalStorage.Constants
{
    public static class FileStorageConstant
    {
        public static readonly string BinaryFilePathTemplate = "{0}.bin";
        public static readonly string FileAlreadyExists = "File already exists for key '{1}'";
        public static readonly string FileNotFound = "File not found for key '{0}'";
        public static readonly string InvalidFileKey = "Invalid file key '{0}'";
        public static readonly string RootPathNotConfigurated = "Root path not configured for storage access";
        public static readonly string UnauthorisedFilePath = "Cannot access file path '{0}'";
    }
}
