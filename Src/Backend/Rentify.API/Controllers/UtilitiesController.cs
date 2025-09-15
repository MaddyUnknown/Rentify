using Microsoft.AspNetCore.Mvc;
using Rentify.Service.DTOs;
using Rentify.Service.Interfaces;

namespace Rentify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UtilitiesController : ControllerBase
{
    private readonly IUtilityService _utilityService;
    private readonly ILogger<UtilitiesController> _logger;

    public UtilitiesController(IUtilityService utilityService, ILogger<UtilitiesController> logger)
    {
        _utilityService = utilityService;
        _logger = logger;
    }

    /// <summary>
    /// Get all utilities
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UtilityDto>>> GetUtilities()
    {
        try
        {
            var utilities = await _utilityService.GetAllUtilitiesAsync();
            return Ok(utilities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving utilities");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get utilities by property ID
    /// </summary>
    [HttpGet("by-property/{propertyId}")]
    public async Task<ActionResult<IEnumerable<UtilityDto>>> GetUtilitiesByProperty(int propertyId)
    {
        try
        {
            var utilities = await _utilityService.GetUtilitiesByPropertyIdAsync(propertyId);
            return Ok(utilities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving utilities for property {PropertyId}", propertyId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get utility by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UtilityDto>> GetUtility(int id)
    {
        try
        {
            var utility = await _utilityService.GetUtilityByIdAsync(id);
            if (utility == null)
            {
                return NotFound($"Utility with ID {id} not found");
            }

            return Ok(utility);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving utility with ID {UtilityId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new utility
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UtilityDto>> CreateUtility([FromBody] CreateUtilityDto createUtilityDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var utility = await _utilityService.CreateUtilityAsync(createUtilityDto);
            return CreatedAtAction(nameof(GetUtility), new { id = utility.UtilityId }, utility);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating utility");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update an existing utility
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<UtilityDto>> UpdateUtility(int id, [FromBody] UpdateUtilityDto updateUtilityDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var utility = await _utilityService.UpdateUtilityAsync(id, updateUtilityDto);
            if (utility == null)
            {
                return NotFound($"Utility with ID {id} not found");
            }

            return Ok(utility);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating utility with ID {UtilityId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a utility
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUtility(int id)
    {
        try
        {
            var deleted = await _utilityService.DeleteUtilityAsync(id);
            if (!deleted)
            {
                return NotFound($"Utility with ID {id} not found");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting utility with ID {UtilityId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
