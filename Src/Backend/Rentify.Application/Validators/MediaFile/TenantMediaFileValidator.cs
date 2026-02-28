using Rentify.Application.Constants;
using Rentify.Application.Interfaces.Validators;
using Rentify.Application.Utils;
using Rentify.Core.Constants;
using Rentify.Core.Entities;
using Rentify.Core.Enums;
using Rentify.DataAccess.Core.Repositories;
using Rentify.FileWorkflow.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Validators.MediaFile
{
    public class TenantMediaFileValidator : IMediaFileValidator
    {
        private readonly ImmutableArray<string> AllowedMimeType = [MediaContentType.Pdf];

        private readonly IRepository<Tenant> _tenantCRUDRepo;

        public TenantMediaFileValidator(IRepository<Tenant> tenantCRUDRepo)
        {
            _tenantCRUDRepo = tenantCRUDRepo;
        }

        public MediaFileEntityEnum EntityType => MediaFileEntityEnum.Tenant;

        public async Task<IEnumerable<string>> ValidateEntityAsync(string fileName, FileInspectionInfo fileInfo, int? entityId = null)
        {
            var errorList = new List<string>();

            if (entityId.HasValue)
            {
                var property = await _tenantCRUDRepo.GetByIdAsync(entityId.Value);
                if (property == null) errorList.Add(string.Format(TenantConstants.TenantNotFound, entityId));
            }

            if (fileInfo.MimeType == null || !AllowedMimeType.Contains(fileInfo.MimeType)) errorList.Add(string.Format(MediaFileConstants.InvalidMediaTypeProvided, string.Join(", ", AllowedMimeType.Select(x => $"'{x}'"))));

            if (fileInfo.MimeType != null && fileInfo.Extension != null && FileExtensionHelper.HasExtension(fileName, out string extension) && !fileInfo.Extension.Contains(extension))
            {
                errorList.Add(string.Format(MediaFileConstants.MediaTypeNotMatchingExtension, fileInfo.MimeType, extension, string.Join(", ", fileInfo.Extension.Select(x => $"'{x}'"))));
            }

            return errorList;
        }
    }
}
