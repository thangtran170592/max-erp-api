using System.IdentityModel.Tokens.Jwt;
using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        [Authorize(Policy = "...")]
        [HttpGet("users")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserResponseDto>>>> GetUsers()
        {
            try
            {
                var sid = CookieHelper.GetClaimValue(Request.Cookies, JwtRegisteredClaimNames.Sid);
                var users = await _chatService.GetChatAbleUsers(Guid.Parse(sid.Value));
                return Ok(ApiResponseHelper.CreateSuccessResponse(users));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("messages/{withUserId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MessageResponseDto>>>> GetMessages(Guid withUserId)
        {
            try
            {
                var sid = CookieHelper.GetClaimValue(Request.Cookies, JwtRegisteredClaimNames.Sid);
                var messages = await _chatService.GetMessages(null, Guid.Parse(sid.Value), withUserId);
                return Ok(ApiResponseHelper.CreateSuccessResponse(messages));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("messages")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequestDto request)
        {
            try
            {
                var sid = CookieHelper.GetClaimValue(Request.Cookies, JwtRegisteredClaimNames.Sid);
                var messages = await _chatService.SendMessage(Guid.Parse(sid.Value), request.ReceiverId, request.RoomId, request.Content, request.Type);
                return Ok(ApiResponseHelper.CreateSuccessResponse(messages));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("rooms")]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequestDto request)
        {
            try
            {
                var sid = CookieHelper.GetClaimValue(Request.Cookies, JwtRegisteredClaimNames.Sid);
                var room = await _chatService.CreateRoom(request.Name, Guid.Parse(sid.Value), request.UserIds);
                return Ok(ApiResponseHelper.CreateSuccessResponse(room));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("rooms")]
        public async Task<IActionResult> GetRooms()
        {
            try
            {
                var sid = CookieHelper.GetClaimValue(Request.Cookies, JwtRegisteredClaimNames.Sid);
                var rooms = await _chatService.GetUserRooms(Guid.Parse(sid.Value));
                return Ok(ApiResponseHelper.CreateSuccessResponse(rooms));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("broadcast")]
        public async Task<IActionResult> SendBroadcast([FromBody] BroadcastRequestDto request)
        {
            try
            {
                var sid = CookieHelper.GetClaimValue(Request.Cookies, JwtRegisteredClaimNames.Sid);
                var result = await _chatService.SendBroadcast(Guid.Parse(sid.Value), request.Content, request.Type, request.TargetType, request.TargetId);
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }
    }
}