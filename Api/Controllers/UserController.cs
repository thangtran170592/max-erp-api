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
        public async Task<ActionResult<ApiResponse<List<UserResponseDto>>>> Filter()
        {
            try
            {
                var columns = new Dictionary<string, object>();
                columns.Add("UserName", "than");
                var result = await _userService.FindManyWithPagingAsync(columns);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in filter User");
                var meta = new MetaData()
                {
                    Page = 1,
                    PageSize = 10,
                    TotalCount = 0,
                    TotalPages = 0,
                };
                return BadRequest(ApiResponseHelper.CreateSuccessResponse<List<UserResponseDto>>([], meta: meta));
            }
        }
    }
}