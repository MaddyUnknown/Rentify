using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.FileWorkflow.Core.DTOs
{
    public class FileInspectionInfo
    {
        public string? MimeType { get; set; }
        public string[]? Extension { get; set; }

        public static FileInspectionInfo Empty => new FileInspectionInfo();
    }
}
