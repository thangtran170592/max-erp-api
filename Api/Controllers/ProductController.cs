using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Authorize]
    public class ProductController : BaseController
    {
        private readonly IProductService _serviceProduct;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService serviceProduct, ILogger<ProductController> logger)
        {
            _serviceProduct = serviceProduct;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<ProductResponseDto>>>> GetAll()
        {
            try
            {
                var data = await _serviceProduct.GetAllAsync();
                return Ok(ApiResponseHelper.CreateSuccessResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetActive()
        {
            try
            {
                var data = await _serviceProduct.GetManyAsync(p => p.Status);
                return Ok(ApiResponseHelper.CreateSuccessResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active products");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("search")]
        public async Task<ActionResult<ApiResponseDto<List<ProductResponseDto>>>> Search([FromBody] FilterRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var departmentId = GetCurrentDepartmentId();
                var positionId = GetCurrentPositionId();
                var result = await _serviceProduct.GetManyWithPagingAsync(userId, departmentId, positionId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<ProductResponseDto>>> GetById(Guid id)
        {
            try
            {
                var item = await _serviceProduct.GetByIdAsync(id);
                if (item == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product by id");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("exists/{uid}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Exists(string uid)
        {
            try
            {
                var result = await _serviceProduct.IsExistAsync(entry => entry.Uid == uid);
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if product exists");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<ProductResponseDto>>> Create([FromBody] ProductRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                dto.CreatedBy = userId;
                var created = await _serviceProduct.CreateAsync(dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<ProductResponseDto>>> Update(Guid id, [FromBody] ProductRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                dto.UpdatedBy = userId;
                dto.UpdatedAt = DateTime.UtcNow;
                var updated = await _serviceProduct.UpdateAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPatch("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<ProductResponseDto>>> UpdateStatus(Guid id, [FromBody] UpdateApprovalRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                dto.UpdatedBy = userId;
                dto.UpdatedAt = DateTime.UtcNow;
                var updated = await _serviceProduct.UpdateStatusAsync(id, dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product status");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<int>>> Delete(Guid id)
        {
            try
            {
                var deleted = await _serviceProduct.DeleteAsync(id, GetCurrentUserId());
                return Ok(ApiResponseHelper.CreateSuccessResponse(deleted));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }
    }
}