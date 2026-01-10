using Rentify.Application.DTOs.MediaFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Interfaces.Services
{
    public interface IMediaFileService
    {
        Task<MediaFileDto> DeleteMediaFileAsync(DeleteMediaFileDto deleteMediaDto, CancellationToken ct);
        Task<IEnumerable<MediaFileDto>> GetMediaFileStatusAsync(IEnumerable<int> ids);
        Task<MediaFileDto> UploadMediaFileAsync(UploadMediaFileDto uploadPropertyMediaDto, CancellationToken ct);
        Task<MediaFileStreamDto> GetMediaFileStreamAsync(MediaFileStreamSearchDto mediaFileSearchDto, CancellationToken ct);
    }
}
