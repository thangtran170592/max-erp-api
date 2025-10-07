using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/approval-features")]
    [Authorize]
    public class ApprovalFeatureController : BaseController
    {
        private readonly IApprovalFeatureService _service;
        private readonly ILogger<ApprovalFeatureController> _logger;

        public ApprovalFeatureController(IApprovalFeatureService service, ILogger<ApprovalFeatureController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<ApprovalFeatureResponseDto>>>> GetAll([FromQuery] Guid? approvalConfigId)
        {
            try
            {
                var data = await _service.GetAllAsync(approvalConfigId);
                return Ok(ApiResponseHelper.CreateSuccessResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting approval features");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("search")]
        public async Task<ActionResult<ApiResponseDto<List<ApprovalFeatureResponseDto>>>> Search([FromBody] FilterRequestDto dto, [FromQuery] Guid? approvalConfigId)
        {
            try
            {
                var result = await _service.GetManyWithPagingAsync(dto, approvalConfigId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching approval features");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<ApprovalFeatureResponseDto>>> GetById(Guid id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                if (item == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting approval feature by id");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("exists/{approvalConfigId:guid}/{uid}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Exists(Guid approvalConfigId, string uid)
        {
            try
            {
                var result = await _service.IsExistAsync(entry => entry.ApprovalConfigId == approvalConfigId && entry.Uid == uid);
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking approval feature exists");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<ApprovalFeatureResponseDto>>> Create([FromBody] ApprovalFeatureRequestDto dto)
        {
            try
            {
                dto.CreatedBy = GetCurrentUserId();
                var created = await _service.CreateAsync(dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating approval feature");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<ApprovalFeatureResponseDto>>> Update(Guid id, [FromBody] ApprovalFeatureRequestDto dto)
        {
            try
            {
                dto.UpdatedBy = GetCurrentUserId();
                var updated = await _service.UpdateAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating approval feature");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPatch("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<ApprovalFeatureResponseDto>>> UpdateStatus(Guid id, [FromBody] UpdateApprovalFeatureStatusRequestDto dto)
        {
            try
            {
                dto.UpdatedBy = GetCurrentUserId();
                var updated = await _service.UpdateStatusAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating approval feature status");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<int>>> Delete(Guid id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id, GetCurrentUserId());
                return Ok(ApiResponseHelper.CreateSuccessResponse(deleted));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting approval feature");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }
    }
}
