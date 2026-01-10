using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.FileWorkflow.Core.Generators
{
    public interface IImageThumbnailGenerator
    {
        public Stream Generate(Stream stream, int targetWidth, int targetHeight);
    }
}
