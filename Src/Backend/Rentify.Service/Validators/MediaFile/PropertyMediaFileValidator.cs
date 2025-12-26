using Rentify.Application.Constants;
using Rentify.Application.Interfaces.Services;
using Rentify.Application.Interfaces.Validators;
using Rentify.Core.Entities;
using Rentify.Core.Enums;
using Rentify.DataAccess.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Validators.MediaFile
{
    public class PropertyMediaFileValidator : IMediaFileValidator
    {
        private readonly IRepository<Property> _propertyCRUDRepo;

        public PropertyMediaFileValidator(IRepository<Property> propertyCRUDRepo)
        {
            _propertyCRUDRepo = propertyCRUDRepo;
        }

        public MediaFileEntityEnum EntityType => MediaFileEntityEnum.Property;

        public async Task<IEnumerable<string>> ValidateEntityAsync(int entityId)
        {
            var errorList = new List<string>();

            var property = await _propertyCRUDRepo.GetByIdAsync(entityId);
            if (property == null) errorList.Add(string.Format(PropertyConstants.PropertyNotFound, entityId));

            return errorList;
        }
    }
}
