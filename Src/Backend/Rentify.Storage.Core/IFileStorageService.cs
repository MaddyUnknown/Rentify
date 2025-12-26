using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Storage.Core
{
    public interface IFileStorageService
    {
        Task<bool> ExistsAsync(string key);
        Task DeleteAsync(string key, CancellationToken ct);
        Task<Stream> ReadAsync(string key, CancellationToken ct);
        Task WriteAsync(string key, Stream content, CancellationToken ct);
    }
}
