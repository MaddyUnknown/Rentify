using Microsoft.AspNetCore.Mvc;
using Rentify.Service.DTOs;
using Rentify.Service.Interfaces;

namespace Rentify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitsController : ControllerBase
{
    private readonly IUnitService _unitService;
    private readonly ILogger<UnitsController> _logger;

    public UnitsController(IUnitService unitService, ILogger<UnitsController> logger)
    {
        _unitService = unitService;
        _logger = logger;
    }

    /// <summary>
    /// Get all units
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnitDto>>> GetUnits()
    {
        try
        {
            var units = await _unitService.GetAllUnitsAsync();
            return Ok(units);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving units");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get units by property ID
    /// </summary>
    [HttpGet("by-property/{propertyId}")]
    public async Task<ActionResult<IEnumerable<UnitDto>>> GetUnitsByProperty(int propertyId)
    {
        try
        {
            var units = await _unitService.GetUnitsByPropertyIdAsync(propertyId);
            return Ok(units);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving units for property {PropertyId}", propertyId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get unit by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UnitDto>> GetUnit(int id)
    {
        try
        {
            var unit = await _unitService.GetUnitByIdAsync(id);
            if (unit == null)
            {
                return NotFound($"Unit with ID {id} not found");
            }

            return Ok(unit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving unit with ID {UnitId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new unit
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UnitDto>> CreateUnit([FromBody] CreateUnitDto createUnitDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var unit = await _unitService.CreateUnitAsync(createUnitDto);
            return CreatedAtAction(nameof(GetUnit), new { id = unit.UnitId }, unit);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating unit");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update an existing unit
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<UnitDto>> UpdateUnit(int id, [FromBody] UpdateUnitDto updateUnitDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var unit = await _unitService.UpdateUnitAsync(id, updateUnitDto);
            if (unit == null)
            {
                return NotFound($"Unit with ID {id} not found");
            }

            return Ok(unit);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating unit with ID {UnitId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a unit
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUnit(int id)
    {
        try
        {
            var deleted = await _unitService.DeleteUnitAsync(id);
            if (!deleted)
            {
                return NotFound($"Unit with ID {id} not found");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting unit with ID {UnitId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
