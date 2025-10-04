using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/packages")]
    [Authorize]
    public class PackageController : BaseController
    {
        private readonly IPackageService _servicePackage;
        private readonly ILogger<PackageController> _logger;

        public PackageController(IPackageService servicePackage, ILogger<PackageController> logger)
        {
            _servicePackage = servicePackage;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<PackageResponseDto>>>> GetAll()
        {
            try
            {
                var data = await _servicePackage.GetAllAsync();
                return Ok(ApiResponseHelper.CreateSuccessResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting packages");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

       [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<PackageResponseDto>>> GetActive()
        {
            try
            {
                var data = await _servicePackage.GetManyAsync(p => p.Status);
                return Ok(ApiResponseHelper.CreateSuccessResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active packages");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("search")]
        public async Task<ActionResult<ApiResponseDto<List<PackageResponseDto>>>> Search([FromBody] FilterRequestDto dto)
        {
            try
            {
                var result = await _servicePackage.GetManyWithPagingAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching packages");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<PackageResponseDto>>> GetById(Guid id)
        {
            try
            {
                var item = await _servicePackage.GetByIdAsync(id);
                if (item == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting package");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }


        [HttpGet("exists/{uid}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Exists(string uid)
        {
            try
            {
                var exists = await _servicePackage.IsExistAsync(pu => pu.Uid == uid);
                return Ok(ApiResponseHelper.CreateSuccessResponse(exists));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking package exists");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<PackageResponseDto>>> Create([FromBody] PackageRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                dto.CreatedBy = userId;
                dto.CreatedAt = DateTime.UtcNow;
                var created = await _servicePackage.CreateAsync(dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating package");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<PackageResponseDto>>> Update(Guid id, [FromBody] PackageRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                dto.UpdatedBy = userId;
                dto.UpdatedAt = DateTime.UtcNow;
                var updated = await _servicePackage.UpdateAsync(id, dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating package");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPatch("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<PackageResponseDto>>> UpdateStatus(Guid id, [FromBody] PackageStatusUpdateDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                dto.UpdatedBy = userId;
                dto.UpdatedAt = DateTime.UtcNow;
                var updated = await _servicePackage.UpdateStatusAsync(id, dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating package status");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<int>>> Delete(Guid id)
        {
            try
            {
                var deleted = await _servicePackage.DeleteAsync(id, GetCurrentUserId());
                return Ok(ApiResponseHelper.CreateSuccessResponse(deleted));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting package");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }
    }
}