using Microsoft.AspNetCore.Mvc;
using Rentify.API.Abstractions.Controllers;
using Rentify.API.DTOs;
using Rentify.Application.DTOs.Location;
using Rentify.Application.DTOs.MediaFile;
using Rentify.Application.DTOs.Property;
using Rentify.Application.DTOs.Unit;
using Rentify.Application.Interfaces.Services;
using Rentify.Core.Enums;

namespace Rentify.API.Controllers;

[ApiController]
[Route("api/properties")]
public class PropertiesController : ApiControllerBase
{
    private readonly IPropertyService _propertyService;
    private readonly IMediaFileService _mediaFileService;
    private readonly IUnitService _unitService;

    private readonly ILogger<PropertiesController> _logger;

    public PropertiesController(IPropertyService propertyService, IUnitService unitService, IMediaFileService mediaFileService, ILogger<PropertiesController> logger)
    {
        _propertyService = propertyService;
        _mediaFileService = mediaFileService;
        _unitService = unitService;
        _logger = logger;
    }

    #region Property Details Endpoint

    /// <summary>
    /// Get property by ID
    /// </summary>
    [HttpGet("{propertyId}/aggregate")]
    public async Task<ActionResult<ResponseWrapper<PropertyDto>>> GetPropertyAggregate(int propertyId)
    {
        try
        {
            var property = await _propertyService.GetPropertyByIdAsync(propertyId);
            return Ok(ResponseWrapper<PropertyDto>.SuccessResponse(property));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving property aggregate for propertyId '{propertyId}'", propertyId);
            return HandleException(ex);
        }
    }

    /// <summary>
    /// Update an existing property
    /// </summary>
    [HttpPut("{propertyId}")]
    public async Task<ActionResult<PropertyDetailsDto>> UpdatePropertyDetails(int propertyId, [FromBody] UpdatePropertyDetailsDto updatePropertyDeatilsDto)
    {
        try
        {
            var property = await _propertyService.UpdatePropertyAsync(propertyId, updatePropertyDeatilsDto);
            return Ok(ResponseWrapper<PropertyDetailsDto>.SuccessResponse(property));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating property for propertyId '{propertyId}'", propertyId);
            return HandleException(ex);
        }
    }

    /// <summary>
    /// Delete a property
    /// </summary>
    [HttpDelete("{propertyId}")]
    public async Task<ActionResult<PropertyDetailsDto>> DeleteProperty(int propertyId)
    {
        try
        {
            var property = await _propertyService.DeletePropertyAsync(propertyId);
            return Ok(ResponseWrapper<PropertyDetailsDto>.SuccessResponse(property));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting property for propertyId '{propertyId}'", propertyId);
            return HandleException(ex);
        }
    }


    /// <summary>
    /// Update property location
    /// </summary>
    [HttpPut("{propertyId}/location")]
    public async Task<ActionResult<PropertyLocationDto>> UpdatePropertyLocation(int propertyId, [FromBody] UpdatePropertyLocationDto updatePropertyLocationDto)
    {
        try
        {
            var location = await _propertyService.UpdatePropertyLocationAsync(propertyId, updatePropertyLocationDto);
            return Ok(ResponseWrapper<PropertyLocationDto>.SuccessResponse(location));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating property location for propertyId '{propertyId}'", propertyId);
            return HandleException(ex);
        }
    }

    #endregion


    #region Property Media Endpoint

    /// <summary>
    /// Upload media file
    /// </summary>
    [HttpPost("{propertyId}/media")]
    public async Task<ActionResult<MediaFileDto>> UploadPropertyMedia(int propertyId, IFormFile file, CancellationToken ct)
    {
        await using Stream stream = file.OpenReadStream();

        try
        {
            var mediaFileDto = new UploadMediaFileDto { MediaStream = stream, Length = file.Length, FileName = file.FileName, EntityId = propertyId, EntityType = MediaFileEntityEnum.Property };
            var result = await _mediaFileService.UploadMediaFileAsync(mediaFileDto, ct);
            return Ok(ResponseWrapper<MediaFileDto>.SuccessResponse(result));
        } 
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error uploading property media for propertyId '{propertyId}'", propertyId);
            return HandleException(ex);
        }
    }

    /// <summary>
    /// Delete media file
    /// </summary>
    [HttpDelete("{propertyId}/media/{mediaId}")]
    public async Task<ActionResult<MediaFileDto>> DeletePropertyMedia(int propertyId, int mediaId, CancellationToken ct)
    {
        try
        {
            var mediaFileDto = new DeleteMediaFileDto { Id = mediaId, EntityId = propertyId, EntityType = MediaFileEntityEnum.Property };
            var deletedMediaFileDto = await _mediaFileService.DeleteMediaFileAsync(mediaFileDto, ct);
            return Ok(ResponseWrapper<MediaFileDto>.SuccessResponse(deletedMediaFileDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting property media for propertyId '{propertyId}', fileMediaId '{mediaId}'", propertyId, mediaId);
            return HandleException(ex);
        }
    }

    #endregion


    #region Property Unit Endpoint

    /// <summary>
    /// Create unit
    /// </summary>
    [HttpPost("{propertyId}/units")]
    public async Task<ActionResult<UnitDto>> CreateUnit(int propertyId, CreateUnitDto createUnitDto)
    {
        try
        {
            var unit = await _unitService.CreateUnitAsync(propertyId, createUnitDto);
            return Ok(ResponseWrapper<UnitDto>.SuccessResponse(unit));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating unit for propertyId '{propertyId}'", propertyId);
            return HandleException(ex);
        }
    }

    /// <summary>
    /// Update an existing unit
    /// </summary>
    [HttpPut("{propertyId}/units/{unitId}")]
    public async Task<ActionResult<UnitDto>> UpdateUnit(int propertyId, int unitId, [FromBody] UpdateUnitDto updateUnitDto)
    {
        try
        {
            var unit = await _unitService.UpdateUnitAsync(unitId, propertyId, updateUnitDto);
            return Ok(ResponseWrapper<UnitDto>.SuccessResponse(unit));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating unit for propertyId {propertyId}, unitId {unitId}", propertyId, unitId);
            return HandleException(ex);
        }
    }

    /// <summary>
    /// Delete a unit
    /// </summary>
    [HttpDelete("{propertyId}/units/{unitId}")]
    public async Task<ActionResult<UnitDto>> DeleteUnit(int propertyId, int unitId)
    {
        try
        {
            var unit = await _unitService.DeleteUnitAsync(unitId, propertyId);
            return Ok(ResponseWrapper<UnitDto>.SuccessResponse(unit));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting unit with propertyId {propertyId}, unitId {unitId}", propertyId, unitId);
            return HandleException(ex);
        }
    }

    #endregion
}
