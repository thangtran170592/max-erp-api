using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Security;
using Application.Dtos;
using Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Utils
{
    public static class JwtToken
    {
        public static TokenResponseDto GenerateJwtToken(ApplicationUser user, IConfiguration config, IList<string> roles)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? string.Empty));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
            };

            if (user.DepartmentId.HasValue)
            {
                claims.Add(new Claim(CustomClaimNames.DepartmentId, user.DepartmentId.Value.ToString()));
            }
            if (user.PositionId.HasValue)
            {
                claims.Add(new Claim(CustomClaimNames.PositionId, user.PositionId.Value.ToString()));
            }

            claims.AddRange(roles.Select(role => new Claim("role", role)));
            var token = new JwtSecurityToken(
            claims: claims,
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials);
            var tokenHandler = new JwtSecurityTokenHandler().WriteToken(token);
            return new TokenResponseDto() { Token = tokenHandler, Expiration = token.ValidTo };
        }

        public static TokenResponseDto GenerateRefreshToken()
        {
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            return new TokenResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddDays(30)
            };
        }
    }
}