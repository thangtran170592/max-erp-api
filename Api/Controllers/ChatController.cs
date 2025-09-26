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

        [HttpGet("conversations")]
        [Authorize(Policy = $"PERMISSION:{Permission.ViewChats}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ConversationResponseDto>>>> GetUserConversations()
        {
            try
            {
                var userId = GetCurrentUserId();
                var conversations = await _chatService.GetUserConversations(userId);
                return Ok(ApiResponseHelper.CreateSuccessResponse(conversations));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user conversations");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("conversations")]
        public async Task<ActionResult<ApiResponse<ConversationResponseDto>>> CreateConversation(
            [FromBody] ConversationRequestDto request)
        {
            try
            {
                request.CreatorId = GetCurrentUserId();
                var conversation = await _chatService.CreateConversation(request);
                return Ok(ApiResponseHelper.CreateSuccessResponse(conversation));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating conversation");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("conversations/{conversationId}/messages")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MessageResponseDto>>>> GetMessages(
            Guid conversationId,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50)
        {
            try
            {
                var messages = await _chatService.GetConversationMessages(conversationId, skip, take);
                return Ok(ApiResponseHelper.CreateSuccessResponse(messages));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting conversation messages");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("messages")]
        public async Task<ActionResult<ApiResponse<MessageResponseDto>>> SendMessage(
            [FromBody] MessageRequestDto request)
        {
            try
            {
                request.SenderId = GetCurrentUserId();
                var message = await _chatService.SendMessage(request);
                return Ok(ApiResponseHelper.CreateSuccessResponse(message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("broadcasts")]
        public async Task<ActionResult<ApiResponse<BroadcastResponseDto>>> SendBroadcast([FromBody] BroadcastRequestDto request)
        {
            try
            {
                var senderId = GetCurrentUserId();
                var broadcast = await _chatService.CreateBroadcastAsync(
                    senderId,
                    request.Content,
                    request.Type,
                    request.UserIds,
                    request.ScheduledAt
                );
                return Ok(ApiResponseHelper.CreateSuccessResponse(broadcast));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending broadcast");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("messages/{messageId}/status")]
        public async Task<ActionResult> UpdateMessageStatus(
            Guid messageId,
            [FromBody] UpdateMessageStatusRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _chatService.UpdateMessageStatus(messageId, userId, request.Status);
                return Ok(ApiResponseHelper.CreateSuccessResponse("Message status updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating message status");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        private Guid GetCurrentUserId()
        {
            var sidClaim = CookieHelper.GetClaimValue(Request.Cookies, JwtRegisteredClaimNames.Sid);
            if (sidClaim == null)
            {
                throw new Exception("Sid claim not found");
            }
            return Guid.Parse(sidClaim.Value);
        }
    }
}