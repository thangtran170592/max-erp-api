using Application.Common.Helpers;
using Application.Dtos;
using Application.IServices;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/barcodes")]
    [Authorize]
    public class BarcodeController : BaseController
    {
        private readonly IBarcodeService _serviceBarcode;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<BarcodeController> _logger;

        public BarcodeController(IBarcodeService serviceBarcode, ApplicationDbContext dbContext, ILogger<BarcodeController> logger)
        {
            _serviceBarcode = serviceBarcode;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] BarcodeRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetCurrentUserId();
                request.UserId = userId;
                var result = await _serviceBarcode.GenerateOneAsync(request, cancellationToken);
                if (result == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }

        [HttpPost("generate-batch")]
        public async Task<IActionResult> Batch([FromBody] BarcodeRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetCurrentUserId();
                request.UserId = userId;
                var result = await _serviceBarcode.GenerateBatchAsync(request, cancellationToken);
                if (result == null) return NotFound();
                return Ok(ApiResponseHelper.CreateSuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                return BadRequest(ApiResponseHelper.CreateFailureResponse<string>(ex));
            }
        }
    }
}