using System.IdentityModel.Tokens.Jwt;
using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> Profile()
        {
            try
            {
                var accessToken = Request.Cookies["AccessToken"];
                if (accessToken == null)
                {
                    return Unauthorized(ApiResponseHelper.CreateFailureResponse<string>(errors:
                        [
                            new ApiError() {Field = "accessToken", Message = "AccessToken is empty"},
                        ]
                    ));
                }
                var handler = new JwtSecurityTokenHandler();
                var accessTokenObj = handler.ReadJwtToken(accessToken);
                var sid = accessTokenObj.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sid);
                if (sid == null)
                {
                    return Unauthorized(ApiResponseHelper.CreateFailureResponse<string>(errors:
                        [
                            new ApiError() {Field = "Sid", Message = "Sid is empty"},
                        ]
                    ));
                }
                var user = await _authService.ProfileAsync(Guid.Parse(sid.Value));
                var roles = accessTokenObj.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();
                user.Roles = roles;
                return Ok(ApiResponseHelper.CreateSuccessResponse(user));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _authService.LoginAsync(request, cancellationToken);
                var _cookieOptions = new CookieOptions
                {
                    Secure = false,
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(10)
                };
                Response.Cookies.Append(nameof(result.AccessToken), result.AccessToken.Token, _cookieOptions);
                var cookieOptions = _cookieOptions;
                cookieOptions.Expires = DateTimeOffset.UtcNow.AddMinutes(30);
                Response.Cookies.Append(nameof(result.RefreshToken), result.RefreshToken.Token, cookieOptions);
                var idAddress = GetIPAddressHelper.GetIPAddress(HttpContext);
                await _authService.CreateRefreshTokenAsync(result.User!.Id, result.RefreshToken.Token, idAddress, cancellationToken);
                return Ok(ApiResponseHelper.CreateSuccessResponse(result.User));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<string>>> RefreshTokenAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var refreshTokenFromCookie = Request.Cookies["RefreshToken"];
                if (string.IsNullOrWhiteSpace(refreshTokenFromCookie) || !await _authService.ValidateRefreshTokenAsync(refreshTokenFromCookie, cancellationToken))
                {
                    return Unauthorized(ApiResponseHelper.CreateFailureResponse<string>(errors:
                        [
                            new ApiError() {Field = "Unauthorized", Message = "Unauthorized"},
                        ]
                    ));
                }
                var _cookieOptions = new CookieOptions
                {
                    Secure = false,
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(10)
                };
                var idAddress = GetIPAddressHelper.GetIPAddress(HttpContext);
                var result = await _authService.RefreshTokenAsync(refreshTokenFromCookie, idAddress, cancellationToken);
                Response.Cookies.Append(nameof(result.AccessToken), result.AccessToken.Token, _cookieOptions);
                var cookieOptions = _cookieOptions;
                cookieOptions.Expires = DateTimeOffset.UtcNow.AddMinutes(30);
                Response.Cookies.Append(nameof(result.RefreshToken), result.RefreshToken.Token, cookieOptions);
                return Ok(ApiResponseHelper.CreateSuccessResponse(result.User));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<int>>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _authService.RegisterAsync(request, cancellationToken);
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.CreateFailureResponse<int>(ex));
            }
        }

        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<int>>> LogoutAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var refreshTokenFromCookie = Request.Cookies["RefreshToken"];
                if (string.IsNullOrWhiteSpace(refreshTokenFromCookie) || !await _authService.ValidateRefreshTokenAsync(refreshTokenFromCookie, cancellationToken))
                {
                    return Unauthorized(ApiResponseHelper.CreateFailureResponse<string>(errors:
                        [
                            new ApiError() {Field = "Unauthorized", Message = "Unauthorized"},
                        ]
                    ));
                }
                var idAddress = GetIPAddressHelper.GetIPAddress(HttpContext);
                var result = await _authService.LogoutAsync(refreshTokenFromCookie, idAddress, cancellationToken);
                Response.Cookies.Delete("AccessToken");
                Response.Cookies.Delete("RefreshToken");
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }
    }
}