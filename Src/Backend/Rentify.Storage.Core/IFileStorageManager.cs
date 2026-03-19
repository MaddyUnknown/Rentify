using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Storage.Core
{
    public interface IFileStorageManager
    {
        Task DeleteAsync(string key, CancellationToken ct = default);
        Task<bool> ExistsAsync(string key);
        Task MoveAsync(string key, string newKey, CancellationToken ct = default);
        Task<Stream> ReadAsync(string key, CancellationToken ct = default);
        Task WriteAsync(string key, Stream content, CancellationToken ct = default);
    }
}
