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
    public class WarehouseController : BaseController
    {
        private readonly IWarehouseService _warehouseService;
        private readonly ILogger<WarehouseController> _logger;

        public WarehouseController(IWarehouseService warehouseService, ILogger<WarehouseController> logger)
        {
            _warehouseService = warehouseService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<WarehouseResponseDto>>>> GetAll()
        {
            try
            {
                var result = await _warehouseService.GetAllAsync();
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user conversations");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("search")]
        public async Task<ActionResult<ApiResponseDto<List<WarehouseResponseDto>>>> Search([FromBody] FilterRequestDto dto)
        {
            try
            {
                var result = await _warehouseService.GetManyWithPagingAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in filter User");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<List<UserResponseDto>>(ex));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<WarehouseResponseDto>>> GetById(Guid id)
        {
            try
            {
                var result = await _warehouseService.GetByIdAsync(id);
                if (result == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting warehouse by id");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("exists/{uid}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Exists(string uid)
        {
            try
            {
                var result = await _warehouseService.IsExistAsync(entry => entry.Uid == uid);
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if warehouse exists");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("{id:guid}/history")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<WarehouseHistoryDto>>>> GetWarehouseHistory(Guid id)
        {
            try
            {
                var result = await _warehouseService.GetWarehouseHistoryAsync(id);
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting warehouse history");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<WarehouseResponseDto>>> Create([FromBody] WarehouseRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                dto.CreatedBy = userId;
                dto.CreatedAt = DateTime.UtcNow;
                var created = await _warehouseService.CreateAsync(dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating warehouse");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<WarehouseResponseDto>>> Update(Guid id, [FromBody] WarehouseRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                dto.UpdatedBy = userId;
                var updated = await _warehouseService.UpdateAsync(id, dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating warehouse");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPatch("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<WarehouseResponseDto>>> UpdateStatus(Guid id, [FromBody] WarehouseStatusUpdateDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                dto.UpdatedBy = userId;
                dto.UpdatedAt = DateTime.UtcNow;
                var updated = await _warehouseService.UpdateStatusAsync(id, dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating warehouse status");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<int>>> Delete(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var deleted = await _warehouseService.DeleteAsync(id, userId);
                return Ok(ApiResponseHelper.CreateSuccessResponse(deleted));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting warehouse");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }
    }
}