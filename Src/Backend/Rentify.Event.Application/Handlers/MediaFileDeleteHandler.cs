using Rentify.Core.Entities;
using Rentify.Core.Events;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.Core.UnitOfWork;
using Rentify.Event.Core;
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
        private readonly IRepository<MediaFile> _mediaFileCRUDRepository;
        private readonly IRepository<MediaFileVariant> _mediaFileVariantCRUDRepository;
        private readonly IMediaFileVariantRepository _mediaFileVariantRepository;
        private readonly IFileStorageService _fileStorageService;

        public MediaFileDeleteHandler(IUnitOfWork unitOfWork, IRepository<MediaFile> mediaFileCRUDRepository, IRepository<MediaFileVariant> mediaFileVariantCRUDRepository, IMediaFileVariantRepository mediaFileVariantRepository, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mediaFileCRUDRepository = mediaFileCRUDRepository;
            _mediaFileVariantCRUDRepository = mediaFileVariantCRUDRepository;
            _mediaFileVariantRepository = mediaFileVariantRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task HandleAsync(MediaFileDeleteEvent message)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                //File variant deletion
                var mediaFileVariants = await _mediaFileVariantRepository.GetMediaFileByMediaFileIdAsync(message.MediaFileId);
                foreach (var mediaFileVariant in mediaFileVariants)
                {
                    await _fileStorageService.DeleteAsync(mediaFileVariant.FileKey);
                    _mediaFileVariantCRUDRepository.Remove(mediaFileVariant);

                    await _unitOfWork.SaveChangesAsync();
                }

                //Original file deletion
                var mediaFile = await _mediaFileCRUDRepository.GetByIdAsync(message.MediaFileId);
                if (mediaFile != null)
                {
                    await _fileStorageService.DeleteAsync(mediaFile.FileKey);
                    _mediaFileCRUDRepository.Remove(mediaFile);

                    await _unitOfWork.SaveChangesAsync();
                }

                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
