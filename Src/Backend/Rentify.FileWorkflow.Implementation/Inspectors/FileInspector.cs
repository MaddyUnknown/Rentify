using MimeDetective;
using MimeDetective.Engine;
using MimeDetective.Storage;
using Rentify.FileWorkflow.Core.DTOs;
using Rentify.FileWorkflow.Core.Inspectors;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.FileWorkflow.Implementation.Inspectors
{
    public class FileInspector : IFileInspector
    {
        IContentInspector _contentInspector;
        public FileInspector()
        {
            _contentInspector = new ContentInspectorBuilder()
            {
                Definitions = MimeDetective.Definitions.DefaultDefinitions.All()
            }
            .Build();
        }

        private static ImmutableArray<Definition> GetAllowedDefinations()
        {
            return [
                .. MimeDetective.Definitions.DefaultDefinitions.FileTypes.Images.PNG(),
                .. MimeDetective.Definitions.DefaultDefinitions.FileTypes.Images.JPEG()
            ];
        }

        public FileInspectionInfo Inspect(Stream stream)
        {
            var contentMatch = _contentInspector.Inspect(stream, true, ContentReader.Min);
            foreach (var match in contentMatch.OrderByDescending(m => m.Points))
            {
                if (match.Type != DefinitionMatchType.Complete) continue;

                var file = match.Definition.File;
                if (string.IsNullOrEmpty(file.MimeType)) continue;

                return new FileInspectionInfo { MimeType = file.MimeType, Extension = file.Extensions.ToArray() };
            }

            return FileInspectionInfo.Empty;
        }
    }
}
