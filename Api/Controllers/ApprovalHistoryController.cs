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
