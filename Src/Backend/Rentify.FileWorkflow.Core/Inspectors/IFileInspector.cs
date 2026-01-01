using Rentify.FileWorkflow.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.FileWorkflow.Core.Inspectors
{
    public interface IFileInspector
    {
        FileInspectionInfo Inspect(Stream stream);
    }
}
