using Microsoft.AspNetCore.Http;

namespace Application.Common.Helpers;
public static class GetIPAddressHelper
{
    public static string GetIPAddress(HttpContext context)
    {
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(ip))
        {
            return ip.Split(',')[0];
        }
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}