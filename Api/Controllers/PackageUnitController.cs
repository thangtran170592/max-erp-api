using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/package-units")]
    [Authorize]
    public class PackageUnitController : BaseController
    {
        private readonly IPackageUnitService _servicePackageUnit;
        private readonly ILogger<PackageUnitController> _logger;

        public PackageUnitController(IPackageUnitService servicePackageUnit, ILogger<PackageUnitController> logger)
        {
            _servicePackageUnit = servicePackageUnit;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<PackageUnitResponseDto>>>> GetAll()
        {
            try
            {
                var data = await _servicePackageUnit.GetAllAsync();
                return Ok(ApiResponseHelper.CreateSuccessResponse(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting package units");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("search")]
        public async Task<ActionResult<ApiResponseDto<List<PackageUnitResponseDto>>>> Search([FromBody] FilterRequestDto dto)
        {
            try
            {
                var result = await _servicePackageUnit.GetManyWithPagingAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching package units");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<PackageUnitResponseDto>>> GetById(Guid id)
        {
            try
            {
                var item = await _servicePackageUnit.GetByIdAsync(id);
                if (item == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting package unit");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpGet("exists/{uid}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Exists(string uid)
        {
            try
            {
                var exists = await _servicePackageUnit.IsExistAsync(pu => pu.Uid == uid);
                return Ok(ApiResponseHelper.CreateSuccessResponse(exists));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking package unit exists");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<PackageUnitResponseDto>>> Create([FromBody] PackageUnitRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                dto.CreatedBy = userId;
                dto.CreatedAt = DateTime.UtcNow;
                var created = await _servicePackageUnit.CreateAsync(dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating package unit");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<PackageUnitResponseDto>>> Update(Guid id, [FromBody] PackageUnitRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                dto.UpdatedBy = userId;
                dto.UpdatedAt = DateTime.UtcNow;
                var updated = await _servicePackageUnit.UpdateAsync(id, dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating package unit");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPatch("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<PackageUnitResponseDto>>> UpdateStatus(Guid id, [FromBody] PackageUnitStatusUpdateDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                dto.UpdatedBy = userId;
                var updated = await _servicePackageUnit.UpdateStatusAsync(id, dto);
                return Ok(ApiResponseHelper.CreateSuccessResponse(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating package unit status");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponseDto<int>>> Delete(Guid id)
        {
            try
            {
                var deleted = await _servicePackageUnit.DeleteAsync(id, GetCurrentUserId());
                return Ok(ApiResponseHelper.CreateSuccessResponse(deleted));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting package unit");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }
    }
}