using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/notification")]
    public class NotificationController : BaseController
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationService _notificationService;

        public NotificationController(ILogger<NotificationController> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        [HttpGet("{id}")]
        public async Task Get(Guid id, CancellationToken cancellationToken)
        {
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var bytes = _notificationService.Get(id);
                    await HttpContext.Response.Body.WriteAsync(bytes, cancellationToken);
                    await HttpContext.Response.Body.FlushAsync(cancellationToken);
                    await Task.Delay(3000, cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Client disconnected");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
        }
    }
}