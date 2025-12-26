using Rentify.Application.Constants;
using Rentify.Application.Interfaces.Resolvers;
using Rentify.Application.Interfaces.Validators;
using Rentify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Resolvers
{
    public class MediaFileValidatorResolver : IMediaFileValidatorResolver
    {
        private IReadOnlyDictionary<MediaFileEntityEnum, IMediaFileValidator> _validators;

        public MediaFileValidatorResolver(IEnumerable<IMediaFileValidator> validators)
        {
            _validators = validators.ToDictionary(v => v.EntityType);
        }

        public IMediaFileValidator Resolve(MediaFileEntityEnum entityType)
        {
            if (!_validators.TryGetValue(entityType, out IMediaFileValidator? validator)) throw new NotSupportedException(string.Format(MediaFileConstants.MediaNotSupported, entityType));
            return validator;
        }
    }
}
