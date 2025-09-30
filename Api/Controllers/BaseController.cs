using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Helpers;
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
    }
}