using Microsoft.AspNetCore.Mvc;
using Rentify.API.DTOs;
using Rentify.Auth.Core.Exceptions;
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
            else if(ex is UserLockedOutException userLockException)
            {
                return BadRequest(ResponseWrapper<object>.ErrorResponse([$"User locked till : {userLockException.LockedTillDateTime}"]));
            }
            else if(ex is AppInvalidCredentialException invalidCred)
            {
                return BadRequest(ResponseWrapper<object>.ErrorResponse([$"Invalid credential"]));
            }
            else if(ex is InvalidRefreshTokenException invalidRefreshToken)
            {
                return Unauthorized(ResponseWrapper<object>.ErrorResponse([$"Invalid refresh token"]));
            }
            else
            {
                return StatusCode(500, ResponseWrapper<object>.ErrorResponse(["Internal server error"]));
            }
        }
    }
}
