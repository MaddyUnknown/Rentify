using Rentify.FileWorkflow.Core.Generators;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.FileWorkflow.Implementation.Generators
{
    public class ImageThumbnailGenerator : IImageThumbnailGenerator
    {
        public Stream Generate(Stream stream, int targetWidth, int targetHeight)
        {
            using var image = Image.Load(stream);

            // Step 1: Calculate scale (no stretch)
            var scale = Math.Max((float)targetWidth / image.Width,(float)targetHeight / image.Height);
            var resizedWidth = (int)Math.Ceiling(image.Width * scale);
            var resizedHeight = (int)Math.Ceiling(image.Height * scale);

            // Step 2: Resize
            image.Mutate(x => x.Resize(resizedWidth, resizedHeight, KnownResamplers.Lanczos3));

            // Step 3: Center crop
            var cropX = Math.Max(0, (image.Width - targetWidth) / 2);
            var cropY = Math.Max(0, (image.Height - targetHeight) / 2);
            image.Mutate(x => x.Crop(new Rectangle(cropX,cropY,targetWidth,targetHeight)));

            // Step 4: Encode JPEG (optimized)
            var output = new MemoryStream();

            var encoder = new JpegEncoder
            {
                Quality = 75,
                SkipMetadata = true
            };

            image.Save(output, encoder);
            output.Position = 0;

            return output;
        }
    }
}
