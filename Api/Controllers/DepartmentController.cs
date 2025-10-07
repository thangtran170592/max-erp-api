using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/departments")]
[Authorize]
public class DepartmentController : BaseController
{
    private readonly IDepartmentService _service;
    private readonly ILogger<DepartmentController> _logger;

    public DepartmentController(IDepartmentService service, ILogger<DepartmentController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<DepartmentResponseDto>>>> GetAll()
    {
        try
        {
            var data = await _service.GetAllAsync();
            return Ok(ApiResponseHelper.CreateSuccessResponse(data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching departments");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<DepartmentResponseDto>>> Create([FromBody] DepartmentRequestDto dto)
    {
        try
        {
            dto.CreatedBy = GetCurrentUserId();
            var created = await _service.CreateAsync(dto);
            return Ok(ApiResponseHelper.CreateSuccessResponse(created));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating department");
            return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
        }
    }
}
