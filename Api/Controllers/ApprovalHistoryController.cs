using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/approval-histories")]
[Authorize]
public class ApprovalHistoryController : BaseController
{
    private readonly IApprovalHistoryService _service;
    private readonly ILogger<ApprovalHistoryController> _logger;

    public ApprovalHistoryController(IApprovalHistoryService service, ILogger<ApprovalHistoryController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<ApprovalHistoryResponseDto>>>> GetAll([FromQuery] Guid? approvalInstanceId)
    {
        try
        {
            var data = await _service.GetAllAsync(approvalInstanceId);
            return Ok(ApiResponseHelper.CreateSuccessResponse(data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting approval actions");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }

    [HttpPost("search")]
    public async Task<ActionResult<ApiResponseDto<List<ApprovalHistoryResponseDto>>>> Search([FromBody] FilterRequestDto dto, [FromQuery] Guid? approvalInstanceId)
    {
        try
        {
            var result = await _service.GetManyWithPagingAsync(dto, approvalInstanceId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching approval actions");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<ApprovalHistoryResponseDto>>> GetById(Guid id)
    {
        try
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(ApiResponseHelper.CreateSuccessResponse(item));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting approval action by id");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }

    [HttpGet("exists/{approvalInstanceId:guid}/{stepOrder:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Exists(Guid approvalInstanceId, int stepOrder)
    {
        try
        {
            var result = await _service.IsExistAsync(a => a.ApprovalDocumentId == approvalInstanceId && a.StepOrder == stepOrder);
            return Ok(ApiResponseHelper.CreateSuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking approval action exists");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<ApprovalHistoryResponseDto>>> Create([FromBody] ApprovalHistoryRequestDto dto)
    {
        try
        {
            dto.CreatedBy = GetCurrentUserId();
            var created = await _service.CreateAsync(dto);
            return Ok(ApiResponseHelper.CreateSuccessResponse(created));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating approval action");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<ApprovalHistoryResponseDto>>> Update(Guid id, [FromBody] ApprovalHistoryRequestDto dto)
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
            _logger.LogError(ex, "Error updating approval action");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<ApiResponseDto<ApprovalHistoryResponseDto>>> UpdateStatus(Guid id, [FromBody] UpdateApprovalActionStatusRequestDto dto)
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
            _logger.LogError(ex, "Error updating approval action status");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }

    [HttpPatch("{id:guid}/order")]
    public async Task<ActionResult<ApiResponseDto<ApprovalHistoryResponseDto>>> UpdateOrder(Guid id, [FromBody] UpdateApprovalActionOrderRequestDto dto)
    {
        try
        {
            dto.UpdatedBy = GetCurrentUserId();
            var updated = await _service.UpdateOrderAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating approval action order");
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
            _logger.LogError(ex, "Error deleting approval action");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }
}
