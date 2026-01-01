using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Utils
{
    public static class FileExtensionHelper
    {
        public static bool HasExtension(string fileName, out string extension)
        {
            extension = Path.GetExtension(fileName);
            extension = (!string.IsNullOrWhiteSpace(extension) && extension[0]=='.') ? extension.Substring(1) : extension;

            return (string.IsNullOrWhiteSpace(extension) || fileName.StartsWith('.')) ? false : true;

        }
    }
}
