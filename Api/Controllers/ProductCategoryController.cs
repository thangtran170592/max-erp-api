using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/product-categories")]
    [Authorize]
    public class ProductCategoryController : BaseController
    {
        private readonly IProductCategoryService _serviceProductCategory;
        private readonly ILogger<ProductCategoryController> _logger;

        public ProductCategoryController(IProductCategoryService serviceProductCategory, ILogger<ProductCategoryController> logger)
        {
            _serviceProductCategory = serviceProductCategory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<ProductCategoryResponseDto>>>> GetAll()
        {
            try
            {
                var data = await _serviceProductCategory.GetAllAsync();
                return Ok(ApiResponseHelper.CreateSuccessResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product categories");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("search")]
        public async Task<ActionResult<ApiResponseDto<List<ProductCategoryResponseDto>>>> Search([FromBody] FilterRequestDto dto)
        {
            try
            {
                var result = await _serviceProductCategory.GetManyWithPagingAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching product categories");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<ProductCategoryResponseDto>>> GetById(Guid id)
        {
            try
            {
                var item = await _serviceProductCategory.GetByIdAsync(id);
                if (item == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product category");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }


        [HttpGet("exists/{uid}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Exists(string uid)
        {
            try
            {
                var result = await _serviceProductCategory.IsExistAsync(entry => entry.Uid == uid);
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if product category exists");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<ProductCategoryResponseDto>>> Create([FromBody] ProductCategoryRequestDto dto)
        {
            try
            {
                var created = await _serviceProductCategory.CreateAsync(dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product category");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<ProductCategoryResponseDto>>> Update(Guid id, [FromBody] ProductCategoryRequestDto dto)
        {
            try
            {
                var updated = await _serviceProductCategory.UpdateAsync(id, dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product category");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPatch("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<ProductCategoryResponseDto>>> UpdateStatus(Guid id, [FromBody] ProductCategoryStatusUpdateDto dto)
        {
            try
            {
                var updated = await _serviceProductCategory.UpdateStatusAsync(id, dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product category status");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<int>>> Delete(Guid id)
        {
            try
            {
                var deleted = await _serviceProductCategory.DeleteAsync(id, GetCurrentUserId());
                return Ok(ApiResponseHelper.CreateSuccessResponse(deleted));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product category");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }
    }
}