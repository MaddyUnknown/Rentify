using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rentify.API.Abstractions.Controllers;
using Rentify.API.DTOs;
using Rentify.API.DTOs.Users;
using Rentify.Application.DTOs.Auth;
using Rentify.Application.DTOs.Tenant;
using Rentify.Application.Interfaces.Services;
using Rentify.Auth.Core.Abstractions.Accessors;
using Rentify.Core.Utils;

namespace Rentify.API.Controllers;

[ApiController]
[Route(BASE_ROUTE)]
public class UserController : ApiControllerBase
{
    public const string BASE_ROUTE = "api/users";

    private const string REFRESH_TOKEN_COOKIE_NAME = "__refresh-cookie";
    private const string REFRESH_TOKEN_PATH_NAME = "RefreshTokenPath";

    private readonly IUserService _userService;
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly LinkGenerator _linkGenerator;

    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, IUserContextAccessor userContextAccessor, LinkGenerator linkGenerator, ILogger<UserController> logger)
    {
        _userService = userService;
        _userContextAccessor = userContextAccessor;
        _linkGenerator = linkGenerator;
        _logger = logger;

    }

    /// <summary>
    /// Register User
    /// </summary>
    [HttpPost("")]
    public async Task<ActionResult<ResponseWrapper<UserDto>>> RegisterUser(UserRegisterDto userDto)
    {
        try
        {
            var user = await _userService.RegisterAsync(userDto);
            return Ok(ResponseWrapper<UserDto>.SuccessResponse(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            return HandleException(ex);
        }
    }

    /// <summary>
    /// Generate Token
    /// </summary>
    [HttpPost("auth")]
    public async Task<ActionResult<ResponseWrapper<UserAccessTokenDto>>> GenerateToken(UserCredentialsDto userCredentials)
    {
        try
        {
            var token = await _userService.LoginAsync(userCredentials);
            var accessToken = new UserAccessTokenDto { Token = token.AccessToken, ExpiresAt = token.AccessTokenExpiresAt };
            var refreshToken = new UserRefreshTokenDto { Token = token.RefreshToken, RememberMe = userCredentials.RememberMe };

            // Attach refresh token as cookie for refresh token
            Response.Cookies.Append(REFRESH_TOKEN_COOKIE_NAME, JsonSerializerHelper.Serialize(refreshToken), new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Path = _linkGenerator.GetPathByName(REFRESH_TOKEN_PATH_NAME),
                Expires = userCredentials.RememberMe ? token.RefreshTokenExpiresAt : null,
            });

            return Ok(ResponseWrapper<UserAccessTokenDto>.SuccessResponse(accessToken));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating token for user");
            return HandleException(ex);
        }
    }

    /// <summary>
    /// Generate Token using refresh token
    /// </summary>
    [HttpPost("auth/refresh", Name = REFRESH_TOKEN_PATH_NAME)]
    public async Task<ActionResult<ResponseWrapper<UserAccessTokenDto>>> GenerateNewUserToken()
    {
        try
        {
            if (!Request.Cookies.TryGetValue(REFRESH_TOKEN_COOKIE_NAME, out string? value) || string.IsNullOrEmpty(value)) return Unauthorized(ResponseWrapper<object>.ErrorResponse([$"Login required"]));

            var refreshToken = JsonSerializerHelper.Deserialize<UserRefreshTokenDto>(value);
            if(refreshToken == null) return Unauthorized(ResponseWrapper<object>.ErrorResponse([$"Login required"]));

            var token = await _userService.RefreshUserTokensAsync(refreshToken.Token);
            var accessToken = new UserAccessTokenDto { Token = token.AccessToken, ExpiresAt = token.AccessTokenExpiresAt };
            refreshToken.Token = token.RefreshToken;

            // attach refresh token as cookie for refresh token
            Response.Cookies.Append(REFRESH_TOKEN_COOKIE_NAME, JsonSerializerHelper.Serialize(refreshToken), new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Path = _linkGenerator.GetPathByName(REFRESH_TOKEN_PATH_NAME),
                Expires = refreshToken.RememberMe ? token.RefreshTokenExpiresAt : null
            });

            return Ok(ResponseWrapper<UserAccessTokenDto>.SuccessResponse(accessToken));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating token for user");
            return HandleException(ex);
        }
    }

    /// <summary>
    /// Logout user
    /// </summary>
    [HttpPost("auth/logout")]
    public ActionResult<ResponseWrapper<object?>> Logout()
    {
        try
        {
            // attach refresh token as cookie for refresh token
            Response.Cookies.Append(REFRESH_TOKEN_COOKIE_NAME, string.Empty, new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Path = _linkGenerator.GetPathByName(REFRESH_TOKEN_PATH_NAME),
                Expires = DateTime.UtcNow.AddDays(-1)
            });

            return Ok(ResponseWrapper<object?>.SuccessResponse(null));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loggin out user");
            return HandleException(ex);
        }
    }

    /// <summary>
    /// Get user data
    /// </summary>
    [Authorize]
    [HttpGet("")]
    public async Task<ActionResult<ResponseWrapper<UserDto>>> GetUserData()
    {
        try
        {
            if(!_userContextAccessor.UserContext.IsAuthenticated || !_userContextAccessor.UserContext.UserId.HasValue) return BadRequest(ResponseWrapper<object>.ErrorResponse(["User not logged in"]));

            var userId = _userContextAccessor.UserContext.UserId.Value;
            var user = await _userService.GetUserByIdAsync(userId);

            return Ok(ResponseWrapper<UserDto>.SuccessResponse(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating token for user");
            return HandleException(ex);
        }
    }
}
