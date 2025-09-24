using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Helpers
{
    public class CookieHelper
    {
        public static JwtSecurityToken GetAccessToken(IRequestCookieCollection cookie)
        {
            var accessToken = cookie["AccessToken"] ?? throw new Exception("Invalid AccessToken");
            var handler = new JwtSecurityTokenHandler();
            var accessTokenObj = handler.ReadJwtToken(accessToken);
            return accessTokenObj;
        }

        public static Claim GetClaimValue(IRequestCookieCollection cookie, string type)
        {
            var accessTokenObj = GetAccessToken(cookie) ?? throw new Exception("Invalid AccessToken");
            var value = accessTokenObj.Claims.FirstOrDefault(c => c.Type == type);
            return value ?? throw new Exception("Invalid Claim");
        }
    }
}