using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/unit-of-measures")]
    [Authorize]
    public class UnitOfMeasureController : BaseController
    {
        private readonly IUnitOfMeasureService _serviceUnitOfMeasure;
        private readonly ILogger<UnitOfMeasureController> _logger;

        public UnitOfMeasureController(IUnitOfMeasureService serviceUnitOfMeasure, ILogger<UnitOfMeasureController> logger)
        {
            _serviceUnitOfMeasure = serviceUnitOfMeasure;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<UnitOfMeasureResponseDto>>>> GetAll()
        {
            try
            {
                var data = await _serviceUnitOfMeasure.GetAllAsync();
                return Ok(ApiResponseHelper.CreateSuccessResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting units");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("search")]
        public async Task<ActionResult<ApiResponseDto<List<UnitOfMeasureResponseDto>>>> Search([FromBody] FilterRequestDto dto)
        {
            try
            {
                var result = await _serviceUnitOfMeasure.GetManyWithPagingAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching units");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<UnitOfMeasureResponseDto>>> GetById(Guid id)
        {
            try
            {
                var item = await _serviceUnitOfMeasure.GetByIdAsync(id);
                if (item == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unit");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }


        [HttpGet("exists/{uid}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Exists(string uid)
        {
            try
            {
                var result = await _serviceUnitOfMeasure.IsExistAsync(entry => entry.Uid == uid);
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if unit exists");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<UnitOfMeasureResponseDto>>> Create([FromBody] UnitOfMeasureRequestDto dto)
        {
            try
            {
                var created = await _serviceUnitOfMeasure.CreateAsync(dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating unit");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<UnitOfMeasureResponseDto>>> Update(Guid id, [FromBody] UnitOfMeasureRequestDto dto)
        {
            try
            {
                var updated = await _serviceUnitOfMeasure.UpdateAsync(id, dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating unit");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPatch("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<UnitOfMeasureResponseDto>>> UpdateStatus(Guid id, [FromBody] UnitOfMeasureStatusUpdateDto dto)
        {
            try
            {
                var updated = await _serviceUnitOfMeasure.UpdateStatusAsync(id, dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating unit status");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<int>>> Delete(Guid id)
        {
            try
            {
                var deleted = await _serviceUnitOfMeasure.DeleteAsync(id, GetCurrentUserId());
                return Ok(ApiResponseHelper.CreateSuccessResponse(deleted));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting unit");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }
    }
}
