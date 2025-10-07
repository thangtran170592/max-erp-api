using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/positions")]
[Authorize]
public class PositionController : BaseController
{
    private readonly IPositionService _service;
    private readonly ILogger<PositionController> _logger;

    public PositionController(IPositionService service, ILogger<PositionController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<PositionResponseDto>>>> GetAll()
    {
        try
        {
            var data = await _service.GetAllAsync();
            return Ok(ApiResponseHelper.CreateSuccessResponse(data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching positions");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<PositionResponseDto>>> Create([FromBody] PositionRequestDto dto)
    {
        try
        {
            dto.CreatedBy = GetCurrentUserId();
            var created = await _service.CreateAsync(dto);
            return Ok(ApiResponseHelper.CreateSuccessResponse(created));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating position");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }
}
