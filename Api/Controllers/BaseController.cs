using System.IdentityModel.Tokens.Jwt;
using Application.Common.Helpers;
using Application.Common.Security;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected Guid GetCurrentUserId()
        {
            var sidClaim = CookieHelper.GetClaimValue(Request.Cookies, JwtRegisteredClaimNames.Sid);
            if (sidClaim == null)
            {
                throw new Exception("Sid claim not found");
            }
            return Guid.Parse(sidClaim.Value);
        }

        protected Guid? GetCurrentDepartmentId()
        {
            var departmentIdClaim = CookieHelper.GetClaimValue(Request.Cookies, CustomClaimNames.DepartmentId);
            if (departmentIdClaim == null)
            {
                return null;
            }
            return Guid.Parse(departmentIdClaim.Value);
        }

        protected Guid? GetCurrentPositionId()
        {
            var positionIdClaim = CookieHelper.GetClaimValue(Request.Cookies, CustomClaimNames.PositionId);
            if (positionIdClaim == null)
            {
                return null;
            }
            return Guid.Parse(positionIdClaim.Value);
        }
    }
}