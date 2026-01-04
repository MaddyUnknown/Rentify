using Rentify.Application.Interfaces.Validators;
using Rentify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Interfaces.Resolvers
{
    public interface IMediaFileValidatorResolver
    {
        IMediaFileValidator Resolve(MediaFileEntityEnum entityType);
    }
}
