using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rentify.API.Abstractions.Controllers;
using Rentify.API.DTOs;
using Rentify.API.Enums;
using Rentify.Application.DTOs;
using Rentify.Application.DTOs.MediaFile;
using Rentify.Application.DTOs.Tenant;
using Rentify.Application.Enums;
using Rentify.Application.Interfaces.Services;
using Rentify.Core.Constants;
using Rentify.Core.Enums;

namespace Rentify.API.Controllers;

[Authorize]
[ApiController]
[Route(BASE_ROUTE)]
public class TenantController : ApiControllerBase
{
    public const string BASE_ROUTE = "api/tenants";

    private readonly ITenantService _tenantService;
    private readonly IMediaFileService _mediaFileService;

    private readonly ILogger<TenantController> _logger;

    public TenantController(ITenantService tenantService, IMediaFileService mediaFileService, ILogger<TenantController> logger)
    {
        _tenantService = tenantService;
        _mediaFileService = mediaFileService;
        _logger = logger;
    }

    #region Tenant Details Endpoint

    /// <summary>
    /// Create tenant
    /// </summary>
    [HttpPost("")]
    public async Task<ActionResult<ResponseWrapper<TenantDto>>> CreateTenant(CreateTenantDto createTenantDto)
    {
        try
        {
            var tenant = await _tenantService.CreateTenantAsync(createTenantDto);
            return Ok(ResponseWrapper<TenantDto>.SuccessResponse(tenant));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tenant");
            return HandleException(ex);
        }
    }

    /// <summary>
    /// Get all tenants
    /// </summary>
    [HttpGet("")]
    public async Task<ActionResult<ResponseWrapper<PaginatedList<TenantSummaryDto>>>> GetAllTenant(int? page, int? pageSize, DateTime? asOfDate)
    {
        try
        {
            var tenants = await _tenantService.GetAllTenantAsync(new TenantSearchDto { AsOfDate = asOfDate?.ToLocalTime() ?? DateTime.Now, CurrentPage = page ?? 1, TotalItemPerPage = pageSize ?? 1000 });
            return Ok(ResponseWrapper<PaginatedList<TenantSummaryDto>>.SuccessResponse(tenants));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tenants for page '{pageNo}', pageSize '{pageSize}', asOfDate '{asOfDate}'", page, pageSize, asOfDate);
            return HandleException(ex);
        }
    }

    /// <summary>
    /// Get tenant by ID
    /// </summary>
    [HttpGet("{tenantId}/aggregate")]
    public async Task<ActionResult<ResponseWrapper<TenantDto>>> GetTenantAggregate(int tenantId)
    {
        try
        {
            var tenant = await _tenantService.GetTenantByIdAsync(tenantId);
            return Ok(ResponseWrapper<TenantDto>.SuccessResponse(tenant));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tenant aggregate for tenantId '{tenantId}'", tenantId);
            return HandleException(ex);
        }
    }

    /// <summary>
    /// Update an existing tenant
    /// </summary>
    [HttpPut("{tenantId}")]
    public async Task<ActionResult<TenantDetailsDto>> UpdateTenantDetails(int tenantId, [FromBody] UpdateTenantDetailsDto updateTenantDeatilsDto)
    {
        try
        {
            var tenant = await _tenantService.UpdateTenantAsync(tenantId, updateTenantDeatilsDto);
            return Ok(ResponseWrapper<TenantDetailsDto>.SuccessResponse(tenant));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tenant for tenantId '{tenantId}'", tenantId);
            return HandleException(ex);
        }
    }

    /// <summary>
    /// Delete a tenant
    /// </summary>
    [HttpDelete("{tenantId}")]
    public async Task<ActionResult<TenantDetailsDto>> DeletTenant(int tenantId)
    {
        try
        {
            var tenant = await _tenantService.DeleteTenantAsync(tenantId);
            return Ok(ResponseWrapper<TenantDetailsDto>.SuccessResponse(tenant));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tenant for tenantId '{tenantId}'", tenantId);
            return HandleException(ex);
        }
    }


    /// <summary>
    /// Update tenant emergency contact
    /// </summary>
    [HttpPut("{tenantId}/emergency-contact/{tenantEmergencyContactId}")]
    public async Task<ActionResult<TenantEmergencyContactDto>> UpdateTenantEmergencyContact(int tenantId, int tenantEmergencyContactId, [FromBody] UpdateTenantEmergencyContactDto updateTenantEmergencyContactDto)
    {
        try
        {
            var tenantEmergencyContact = await _tenantService.UpdateTenantEmergencyContactAsync(tenantId, tenantEmergencyContactId, updateTenantEmergencyContactDto);
            return Ok(ResponseWrapper<TenantEmergencyContactDto>.SuccessResponse(tenantEmergencyContact));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tenant emergency contract for tenantId '{tenantId}', tenantEmergencyContactId '{tenantEmergencyContactId}'", tenantId, tenantEmergencyContactId);
            return HandleException(ex);
        }
    }

    #endregion


    #region Tenant Media Endpoint
    /// <summary>
    /// Upload profile pic
    /// </summary>
    [HttpPost("profile-pic")]
    public async Task<ActionResult<MediaFileDto>> UploadTenantProfilePic(IFormFile file, CancellationToken ct)
    {
        await using Stream stream = file.OpenReadStream();

        try
        {
            var mediaFileDto = new UpdateTenantProfilePicDto
            {
                MediaStream = stream,
                Length = file.Length,
                FileName = file.FileName
            };

            var result = await _tenantService.UpdateProfilePic(mediaFileDto, ct);
            return Ok(ResponseWrapper<MediaFileDto>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading tenant profile pic media");
            return HandleException(ex);
        }
    }

    /// <summary>
    /// Upload profile pic
    /// </summary>
    [HttpPost("{tenantId}/profile-pic")]
    public async Task<ActionResult<MediaFileDto>> UploadTenantProfilePic(IFormFile file, int tenantId, CancellationToken ct)
    {
        await using Stream stream = file.OpenReadStream();

        try
        {
            var mediaFileDto = new UpdateTenantProfilePicDto
            {
                TenantId = tenantId,
                MediaStream = stream,
                Length = file.Length,
                FileName = file.FileName
            };

            var result = await _tenantService.UpdateProfilePic(mediaFileDto, ct);
            return Ok(ResponseWrapper<MediaFileDto>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading tenant profile pic media for tenantId '{tenantId}'", tenantId);
            return HandleException(ex);
        }
    }


    /// <summary>
    /// Upload media file
    /// </summary>
    [HttpPost("{tenantId}/documents")]
    public async Task<ActionResult<MediaFileDto>> UploadTenantDocumentMedia(int tenantId, IFormFile file, CancellationToken ct)
    {
        await using Stream stream = file.OpenReadStream();

        try
        {
            var mediaFileDto = new UploadMediaFileDto { 
                MediaStream = stream, 
                Length = file.Length, 
                FileName = file.FileName, 
                EntityId = tenantId, 
                EntityType = MediaFileEntityEnum.Tenant,
                AcceptedContentTypes = [MediaContentType.Pdf]
            };

            var result = await _mediaFileService.UploadMediaFileAsync(mediaFileDto, ct);
            return Ok(ResponseWrapper<MediaFileDto>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading tenant document media for tenantId '{tenantId}'", tenantId);
            return HandleException(ex);
        }
    }

    /// <summary>
    /// Upload media file
    /// </summary>
    [HttpPost("documents")]
    public async Task<ActionResult<MediaFileDto>> UploadTenantDocumentMedia(IFormFile file, CancellationToken ct)
    {
        await using Stream stream = file.OpenReadStream();

        try
        {
            var mediaFileDto = new UploadMediaFileDto { 
                MediaStream = stream, 
                Length = file.Length, 
                FileName = file.FileName, 
                EntityType = MediaFileEntityEnum.Tenant,
                AcceptedContentTypes = [MediaContentType.Pdf]
            };

            var result = await _mediaFileService.UploadMediaFileAsync(mediaFileDto, ct);
            return Ok(ResponseWrapper<MediaFileDto>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading tenant document media");
            return HandleException(ex);
        }
    }

    /// <summary>
    /// Get media file
    /// </summary>
    [HttpGet("media/{mediaId}")]
    public async Task<IActionResult> GetTenantMediaStream(int mediaId, [FromQuery] MediaFileVariantDtoEnum? variantType, CancellationToken ct)
    {
        try
        {
            var mediaFileDto = new MediaFileStreamSearchDto { 
                Id = mediaId, 
                EntityType = MediaFileEntityEnum.Tenant, 
                VariantType = variantType.HasValue ? MediaFileVariantDtoEnumConverter.ToMediaFilVariantEnum(variantType.Value) : null };
            var result = await _mediaFileService.GetMediaFileStreamAsync(mediaFileDto, ct);

            Response.Headers["X-Content-Type-Options"] = "nosniff";
            return File(result.MediaStream, result.ContentType, result.FileName, enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tenant media stram for fileMediaId '{mediaId}'", mediaId);
            return HandleException(ex);
        }
    }

    /// <summary>
    /// Delete media file
    /// </summary>
    [HttpDelete("media/{mediaId}")]
    public async Task<ActionResult<MediaFileDto>> DeleteTenantMedia(int mediaId, CancellationToken ct)
    {
        try
        {
            var mediaFileDto = new DeleteMediaFileDto { Id = mediaId, EntityType = MediaFileEntityEnum.Tenant };
            var deletedMediaFileDto = await _mediaFileService.DeleteMediaFileAsync(mediaFileDto, ct);
            return Ok(ResponseWrapper<MediaFileDto>.SuccessResponse(deletedMediaFileDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tenant media for fileMediaId '{mediaId}'", mediaId);
            return HandleException(ex);
        }
    }

    #endregion
}
