using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/approval-configs")]
    [Authorize]
    public class ApprovalConfigController : BaseController
    {
        private readonly IApprovalConfigService _serviceApprovalConfig;
        private readonly ILogger<ApprovalConfigController> _logger;

        public ApprovalConfigController(IApprovalConfigService serviceApprovalConfig, ILogger<ApprovalConfigController> logger)
        {
            _serviceApprovalConfig = serviceApprovalConfig;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<ApprovalConfigResponseDto>>>> GetAll()
        {
            try
            {
                var data = await _serviceApprovalConfig.GetAllAsync();
                return Ok(ApiResponseHelper.CreateSuccessResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting approval configs");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }


       [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ApprovalConfigResponseDto>>> GetActive()
        {
            try
            {
                var data = await _serviceApprovalConfig.GetManyAsync(p => p.Status);
                return Ok(ApiResponseHelper.CreateSuccessResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active approval configs");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("search")]
        public async Task<ActionResult<ApiResponseDto<List<ApprovalConfigResponseDto>>>> Search([FromBody] FilterRequestDto dto)
        {
            try
            {
                var result = await _serviceApprovalConfig.GetManyWithPagingAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching approval configs");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<ApprovalConfigResponseDto>>> GetById(Guid id)
        {
            try
            {
                var item = await _serviceApprovalConfig.GetByIdAsync(id);
                if (item == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting approval config by id");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("exists/{uid}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Exists(string uid)
        {
            try
            {
                var result = await _serviceApprovalConfig.IsExistAsync(entry => entry.Uid == uid);
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking approval config exists");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<ApprovalConfigResponseDto>>> Create([FromBody] ApprovalConfigRequestDto dto)
        {
            try
            {
                dto.CreatedBy = GetCurrentUserId();
                var created = await _serviceApprovalConfig.CreateAsync(dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating approval config");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<ApprovalConfigResponseDto>>> Update(Guid id, [FromBody] ApprovalConfigRequestDto dto)
        {
            try
            {
                dto.UpdatedBy = GetCurrentUserId();
                var updated = await _serviceApprovalConfig.UpdateAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating approval config");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPatch("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<ApprovalConfigResponseDto>>> UpdateStatus(Guid id, [FromBody] UpdateApprovalConfigStatusRequestDto dto)
        {
            try
            {
                dto.UpdatedBy = GetCurrentUserId();
                var updated = await _serviceApprovalConfig.UpdateStatusAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating approval config status");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<int>>> Delete(Guid id)
        {
            try
            {
                var deleted = await _serviceApprovalConfig.DeleteAsync(id, GetCurrentUserId());
                return Ok(ApiResponseHelper.CreateSuccessResponse(deleted));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting approval config");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }
    }
}
