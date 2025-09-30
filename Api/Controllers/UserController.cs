using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("filter")]
        public async Task<ActionResult<ApiResponseDto<List<UserResponseDto>>>> Filter([FromBody] FilterRequestDto dto)
        {
            try
            {
                var result = await _userService.FindManyWithPagingAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in filter User");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<List<UserResponseDto>>(ex));
            }
        }
    }
}