using System.IdentityModel.Tokens.Jwt;
using Application.Common.Helpers;
using Application.Common.Security;
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

        [Authorize(Policy = $"PERMISSION:{Permission.ViewChats}")]
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

        [HttpPost("history-messages")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MessageResponseDto>>>> HistoryMessages(HistoryMessagesRequestDto request)
        {
            try
            {
                var sid = CookieHelper.GetClaimValue(Request.Cookies, JwtRegisteredClaimNames.Sid);
                var messages = await _chatService.GetMessages(request.RoomId, Guid.Parse(sid.Value), request.CustomerId);
                return Ok(ApiResponseHelper.CreateSuccessResponse(messages));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [Authorize(Policy = $"PERMISSION:{Permission.CreateChats}")]
        [HttpPost("messages")]
        public async Task<IActionResult> SendMessage([FromBody] MessageRequestDto request)
        {
            try
            {
                var sid = CookieHelper.GetClaimValue(Request.Cookies, JwtRegisteredClaimNames.Sid);
                request.SenderId = Guid.Parse(sid.Value);
                var messages = await _chatService.SendMessage(request);
                return Ok(ApiResponseHelper.CreateSuccessResponse(messages));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("rooms")]
        public async Task<ActionResult<RoomResponseDto>> CreateRoom([FromBody] RoomRequestDto request)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.RoomCode))
                {
                    var findRoom = await _chatService.ValidateRoom(request.RoomCode, request.IsGroup);
                    if (findRoom is not null)
                    {
                        return Ok(ApiResponseHelper.CreateSuccessResponse(findRoom));
                    }
                }
                var sid = CookieHelper.GetClaimValue(Request.Cookies, JwtRegisteredClaimNames.Sid);
                var room = await _chatService.CreateRoom(request);
                return Ok(ApiResponseHelper.CreateSuccessResponse(room));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("rooms")]
        public async Task<ActionResult<IEnumerable<RoomResponseDto>>> GetRooms()
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
        public async Task<ActionResult<List<Guid>>> SendBroadcast([FromBody] BroadcastRequestDto request)
        {
            try
            {
                var sid = CookieHelper.GetClaimValue(Request.Cookies, JwtRegisteredClaimNames.Sid);
                var sidName = CookieHelper.GetClaimValue(Request.Cookies, JwtRegisteredClaimNames.Name);
                request.SenderId = Guid.Parse(sid.Value);
                request.SenderName = sidName.Value;
                var result = await _chatService.SendBroadcast(request);
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }
    }
}