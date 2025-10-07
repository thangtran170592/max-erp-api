using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/approval-steps")]
    [Authorize]
    public class ApprovalStepController : BaseController
    {
        private readonly IApprovalStepService _service;
        private readonly ILogger<ApprovalStepController> _logger;

        public ApprovalStepController(IApprovalStepService service, ILogger<ApprovalStepController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<ApprovalStepResponseDto>>>> GetAll([FromQuery] Guid? approvalFeatureId)
        {
            try
            {
                var data = await _service.GetAllAsync(approvalFeatureId);
                return Ok(ApiResponseHelper.CreateSuccessResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting approval steps");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("search")]
        public async Task<ActionResult<ApiResponseDto<List<ApprovalStepResponseDto>>>> Search([FromBody] FilterRequestDto request, [FromQuery] Guid? approvalFeatureId)
        {
            try
            {
                var result = await _service.GetManyWithPagingAsync(request, approvalFeatureId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching approval steps");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<ApprovalStepResponseDto>>> GetById(Guid id)
        {
            try
            {
                var entity = await _service.GetByIdAsync(id);
                if (entity == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting approval step by id");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<ApprovalStepResponseDto>>> Create([FromBody] ApprovalStepRequestDto request)
        {
            try
            {
                request.CreatedBy = GetCurrentUserId();
                var created = await _service.CreateAsync(request);
                return Ok(ApiResponseHelper.CreateSuccessResponse(created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating approval step");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<ApprovalStepResponseDto>>> Update(Guid id, [FromBody] ApprovalStepRequestDto request)
        {
            try
            {
                request.UpdatedBy = GetCurrentUserId();
                var updated = await _service.UpdateAsync(id, request);
                if (updated == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating approval step");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPatch("{id:guid}/order")]
        public async Task<ActionResult<ApiResponseDto<ApprovalStepResponseDto>>> UpdateOrder(Guid id, [FromBody] UpdateApprovalStepOrderRequestDto request)
        {
            try
            {
                request.UpdatedBy = GetCurrentUserId();
                var updated = await _service.UpdateOrderAsync(id, request);
                if (updated == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating approval step order");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPatch("{id:guid}/final-step")]
        public async Task<ActionResult<ApiResponseDto<ApprovalStepResponseDto>>> UpdateIsFinal(Guid id, [FromBody] UpdateApprovalStepStatusRequestDto request)
        {
            try
            {
                request.UpdatedBy = GetCurrentUserId();
                var updated = await _service.UpdateIsFinalAsync(id, request);
                if (updated == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating approval step final flag");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<int>>> Delete(Guid id)
        {
            try
            {
                var result = await _service.DeleteAsync(id, GetCurrentUserId());
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting approval step");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }
    }
}
