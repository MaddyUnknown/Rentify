using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Storage.Core
{
    public interface IFileStorageService
    {
        Task DeleteAsync(string key, CancellationToken ct);
        Task<bool> ExistsAsync(string key);
        Task MoveAsync(string key, string newKey, CancellationToken ct);
        Task<Stream> ReadAsync(string key, CancellationToken ct);
        Task WriteAsync(string key, Stream content, CancellationToken ct);
    }
}
