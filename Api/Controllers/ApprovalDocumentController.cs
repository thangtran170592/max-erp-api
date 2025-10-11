using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/approval-requests")]
[Authorize]
public class ApprovalDocumentController : BaseController
{
    private readonly IApprovalDocumentService _service;
    private readonly ILogger<ApprovalDocumentController> _logger;

    public ApprovalDocumentController(IApprovalDocumentService service, ILogger<ApprovalDocumentController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<ApprovalDocumentResponseDto>>>> GetAll([FromQuery] Guid? approvalFeatureId)
    {
        try
        {
            var data = await _service.GetAllAsync(approvalFeatureId);
            return Ok(ApiResponseHelper.CreateSuccessResponse(data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting approval instances");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }

    [HttpPost("search")]
    public async Task<ActionResult<ApiResponseDto<List<ApprovalDocumentResponseDto>>>> Search([FromBody] FilterRequestDto dto, [FromQuery] Guid? approvalFeatureId, [FromQuery] Guid? dataId)
    {
        try
        {
            var result = await _service.GetManyWithPagingAsync(dto, approvalFeatureId, dataId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching approval instances");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<ApprovalDocumentResponseDto>>> GetById(Guid id)
    {
        try
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(ApiResponseHelper.CreateSuccessResponse(item));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting approval instance by id");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }

    [HttpGet("exists/{approvalFeatureId:guid}/{dataId:guid}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Exists(Guid approvalFeatureId, Guid dataId)
    {
        try
        {
            var result = await _service.IsExistAsync(a => a.ApprovalFeatureId == approvalFeatureId && a.DocumentId == dataId && a.ApprovalStatus != Core.Enums.ApprovalStatus.Rejected);
            return Ok(ApiResponseHelper.CreateSuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking approval instance exists");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<ApprovalDocumentResponseDto>>> Create([FromBody] ApprovalDocumentDto dto)
    {
        try
        {
            dto.CreatedBy = GetCurrentUserId();
            var created = await _service.CreateAsync(dto);
            return Ok(ApiResponseHelper.CreateSuccessResponse(created));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating approval instance");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<ApprovalDocumentResponseDto>>> Update(Guid id, [FromBody] ApprovalDocumentDto dto)
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
            _logger.LogError(ex, "Error updating approval instance");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<ApiResponseDto<ApprovalDocumentResponseDto>>> UpdateStatus(Guid id, [FromBody] UpdateApprovalDocumentStatusRequestDto dto)
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
            _logger.LogError(ex, "Error updating approval instance status");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }

    [HttpPatch("{id:guid}/current-step")]
    public async Task<ActionResult<ApiResponseDto<ApprovalDocumentResponseDto>>> UpdateCurrentStep(Guid id, [FromBody] UpdateApprovalDocumentCurrentStepRequestDto dto)
    {
        try
        {
            dto.UpdatedBy = GetCurrentUserId();
            var updated = await _service.UpdateCurrentStepAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating approval instance current step");
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
            _logger.LogError(ex, "Error deleting approval instance");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }
}
