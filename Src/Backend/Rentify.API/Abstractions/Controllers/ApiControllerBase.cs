using Microsoft.AspNetCore.Mvc;
using Rentify.API.DTOs;
using Rentify.Core.Exceptions;

namespace Rentify.API.Abstractions.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected ObjectResult HandleException(Exception ex)
        {
            if (ex is AppValidationException valException)
            {
                return BadRequest(ResponseWrapper<object>.ErrorResponse(valException.Errors));
            }
            else
            {
                return StatusCode(500, ResponseWrapper<object>.ErrorResponse(new List<string> { "Internal server error" }));
            }
        }
    }
}
