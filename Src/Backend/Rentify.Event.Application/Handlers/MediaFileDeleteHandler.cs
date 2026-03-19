using Rentify.Core.Entities;
using Rentify.Core.Enums;
using Rentify.Core.Events;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.Core.UnitOfWork;
using Rentify.Event.Application.Constants;
using Rentify.Event.Core;
using Rentify.Event.Core.Contexts;
using Rentify.Storage.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Application.Handlers
{
    public class MediaFileDeleteHandler : IMessageHandler<MediaFileDeleteEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<MediaFileLink> _mediaFileLinkRepository;
        private readonly IRepository<MediaFile> _mediaFileCRUDRepository;
        private readonly IMediaFileRepository _mediaFileRepository;
        private readonly IRepository<MediaFileVariant> _mediaFileVariantCRUDRepository;
        private readonly IFileStorageManager _fileStorageService;

        public MediaFileDeleteHandler(IUnitOfWork unitOfWork, IRepository<MediaFileLink> mediaFileLink, IRepository<MediaFile> mediaFileCRUDRepository, IRepository<MediaFileVariant> mediaFileVariantCRUDRepository, IMediaFileRepository mediaFileRepository, IFileStorageManager fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mediaFileLinkRepository = mediaFileLink;
            _mediaFileCRUDRepository = mediaFileCRUDRepository;
            _mediaFileRepository = mediaFileRepository;
            _mediaFileVariantCRUDRepository = mediaFileVariantCRUDRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task HandleAsync(MediaFileDeleteEvent message, IMessageProcessingContext context)
        {
            var mediaFile = await _mediaFileRepository.GetMediaFileByIdAsync(message.MediaFileId);
            if (mediaFile == null) return;
            if (mediaFile.Status != MediaFileStatusEnum.Deleted) throw new InvalidOperationException(string.Format(MediaFileConstants.MediaFileDeleteForStatusError, mediaFile.Id, mediaFile.Status));

            //File link deletion
            if (mediaFile.MediaFileLink != null)
            {
                _mediaFileLinkRepository.Remove(mediaFile.MediaFileLink);
                await _unitOfWork.SaveChangesAsync();
            }

            //File variant deletion
            foreach (var mediaFileVariant in mediaFile.MediaFileVariants)
            {
                await _fileStorageService.DeleteAsync(mediaFileVariant.FileKey);
                _mediaFileVariantCRUDRepository.Remove(mediaFileVariant);
                await _unitOfWork.SaveChangesAsync();
            }

            //Original file deletion
            if (mediaFile != null)
            {
                await _fileStorageService.DeleteAsync(mediaFile.FileKey);
                _mediaFileCRUDRepository.Remove(mediaFile);

                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
