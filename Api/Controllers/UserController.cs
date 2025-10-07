using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<UserResponseDto>>>> GetAll()
        {
            try
            {
                var result = await _userService.GetAll();
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("search")]
        public async Task<ActionResult<ApiResponseDto<List<UserResponseDto>>>> Search([FromBody] FilterRequestDto dto)
        {
            try
            {
                var result = await _userService.FindManyWithPagingAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in search User");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<List<UserResponseDto>>(ex));
            }
        }

        [AllowAnonymous]
        [HttpGet("exists/{uid}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Exists(string uid)
        {
            try
            {
                var result = await _userService.IsExistAsync(entry => entry.UserName == uid);
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user exists");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }
    }
}