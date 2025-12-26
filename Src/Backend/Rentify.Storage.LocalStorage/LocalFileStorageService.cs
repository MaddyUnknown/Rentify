using Rentify.Storage.Core;
using Rentify.Storage.LocalStorage.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Storage.LocalStorage
{
    public class LocalFileStorageService : IFileStorageService
    {
        private string _rootPath;
        private int _bufferSize;

        public LocalFileStorageService(string rootPath, int bufferSize = 81920)
        {
            _rootPath = Path.GetFullPath(rootPath);
            _bufferSize = bufferSize;
        }

        public Task<bool> ExistsAsync(string key)
        {
            var filePath = ResolvePath(key);
            return Task.FromResult(File.Exists(filePath));
        }

        public Task DeleteAsync(string key, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var filePath = ResolvePath(key);
            if (File.Exists(filePath)) File.Delete(filePath);

            return Task.CompletedTask;
        }

        public Task<Stream> ReadAsync(string key, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var filePath = ResolvePath(key);
            if (!File.Exists(filePath)) throw new FileNotFoundException(string.Format(FileStorageConstant.FileNotFound, key), key);

            Stream stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: _bufferSize,
                useAsync: true);

            return Task.FromResult(stream);
        }

        public async Task WriteAsync(string key, Stream content, CancellationToken ct)
        {
            var filePath = ResolvePath(key);
            if (File.Exists(filePath)) throw new InvalidOperationException(string.Format(FileStorageConstant.FileAlreadyExists, key));

            var directory = Path.GetDirectoryName(filePath)!;
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            await using var fileStream = new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: _bufferSize,
                useAsync: true);

            await content.CopyToAsync(fileStream, ct);

        }

        private string ResolvePath(string key)
        {
            if(string.IsNullOrWhiteSpace(key)) throw new ArgumentException(string.Format(FileStorageConstant.InvalidFileKey, key), nameof(key));

            var combined = Path.Combine(_rootPath, string.Format(FileStorageConstant.BinaryFilePathTemplate, key));
            var fullPath = Path.GetFullPath(combined);

            // Prevent path traversal
            if (!fullPath.StartsWith(_rootPath)) throw new UnauthorizedAccessException(string.Format(FileStorageConstant.UnauthorisedFilePath, fullPath));

            return fullPath;
        }
    }
}
