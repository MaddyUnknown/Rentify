using Microsoft.AspNetCore.Mvc;
using Rentify.API.Abstractions.Controllers;
using Rentify.API.DTOs;
using Rentify.Application.DTOs.MediaFile;
using Rentify.Application.DTOs.Property;
using Rentify.Application.Interfaces.Services;

namespace Rentify.API.Controllers
{
    [ApiController]
    [Route("api/media")]
    public class MediaController : ApiControllerBase
    {
        private readonly IMediaFileService _mediaFileService;

        private readonly ILogger<MediaController> _logger;

        public MediaController(IMediaFileService mediaFileService, ILogger<MediaController> logger)
        {
            _mediaFileService = mediaFileService;
            _logger = logger;
        }

        /// <summary>
        /// Get property by ID
        /// </summary>
        [HttpPost("polling")]
        public async Task<ActionResult<ResponseWrapper<IEnumerable<MediaFileDto>>>> GetMediaFileStatus(IEnumerable<int> mediaFileIds)
        {
            try
            {
                var mediaFiles = await _mediaFileService.GetMediaFileStatusAsync(mediaFileIds);
                return Ok(ResponseWrapper<IEnumerable<MediaFileDto>>.SuccessResponse(mediaFiles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving property aggregate for mediaFileIds '{mediaFileIds}'", mediaFileIds.ToArray());
                return HandleException(ex);
            }
        }
    }
}
