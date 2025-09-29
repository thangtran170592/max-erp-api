using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/warehouses")]
    [Authorize]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _service;
        private readonly ILogger<WarehouseController> _logger;

        public WarehouseController(IWarehouseService service, ILogger<WarehouseController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<WarehouseResponseDto>>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user conversations");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<WarehouseResponseDto>>> GetById(Guid id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                if (result == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting warehouse by id");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<WarehouseResponseDto>>> Create([FromBody] WarehouseRequestDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating warehouse");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse<WarehouseResponseDto>>> Update(Guid id, [FromBody] WarehouseRequestDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating warehouse");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Nếu bạn có phương thức DeleteAsync trong service thì dùng:
            // var deleted = await _service.DeleteAsync(id);
            // if (!deleted) return NotFound();
            // return NoContent();
            return StatusCode(501); // Not Implemented
        }
    }
}