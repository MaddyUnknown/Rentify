using Rentify.Application.DTOs.MediaFile;
using Rentify.Core.Enums;
using Rentify.FileWorkflow.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Interfaces.Validators
{
    public interface IMediaFileValidator
    {
        MediaFileEntityEnum EntityType { get; }

        Task<IEnumerable<string>> ValidateEntityAsync(int entityId, string fileName, FileInspectionInfo fileInfo);
    }
}
