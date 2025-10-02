using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/suppliers")]
    [Authorize]
    public class SupplierController : BaseController
    {
        private readonly ISupplierService _serviceSupplier;
        private readonly ILogger<SupplierController> _logger;

        public SupplierController(ISupplierService serviceSupplier, ILogger<SupplierController> logger)
        {
            _serviceSupplier = serviceSupplier;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<SupplierResponseDto>>>> GetAll()
        {
            try
            {
                var data = await _serviceSupplier.GetAllAsync();
                return Ok(ApiResponseHelper.CreateSuccessResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting suppliers");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("search")]
        public async Task<ActionResult<ApiResponseDto<List<SupplierResponseDto>>>> Search([FromBody] FilterRequestDto dto)
        {
            try
            {
                var result = await _serviceSupplier.GetManyWithPagingAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching suppliers");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<SupplierResponseDto>>> GetById(Guid id)
        {
            try
            {
                var item = await _serviceSupplier.GetByIdAsync(id);
                if (item == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting supplier");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }


        [HttpGet("exists/{uid}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Exists(string uid)
        {
            try
            {
                var result = await _serviceSupplier.IsExistAsync(entry => entry.Uid == uid);
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if supplier exists");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<SupplierResponseDto>>> Create([FromBody] SupplierRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                dto.CreatedBy = userId;
                dto.CreatedAt = DateTime.UtcNow;
                var created = await _serviceSupplier.CreateAsync(dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating supplier");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<SupplierResponseDto>>> Update(Guid id, [FromBody] SupplierRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                dto.UpdatedBy = userId;
                dto.UpdatedAt = DateTime.UtcNow;
                var updated = await _serviceSupplier.UpdateAsync(id, dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating supplier");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPatch("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<SupplierResponseDto>>> UpdateStatus(Guid id, [FromBody] SupplierStatusUpdateDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                dto.UpdatedBy = userId;
                var updated = await _serviceSupplier.UpdateStatusAsync(id, dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating supplier status");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<int>>> Delete(Guid id)
        {
            try
            {
                var deleted = await _serviceSupplier.DeleteAsync(id, GetCurrentUserId());
                return Ok(ApiResponseHelper.CreateSuccessResponse(deleted));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting supplier");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }
    }
}