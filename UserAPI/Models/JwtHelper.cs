using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UserAPI.Models
{
    public static class JwtHelper
    {
        public static string GenerateJwtToken(Guid id, string role)
        {
            DateTime utcNow = DateTime.UtcNow;
            JwtSecurityToken? token = new JwtSecurityToken(
                issuer: "user-api-issuer",
                audience: "user-api-audience",
                notBefore: utcNow,
                claims: CreateClaims(id, role).Claims,
                expires: utcNow.Add(TimeSpan.FromMinutes(25)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("c2Rvdmpvc2lkam5qZ293am8xMjM0MzI0NW8yaTM0NTFvMzVpMzJ1NjB1Mm9paGpyb3V3ZjlodzM0OXJoajk=")), SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static ClaimsIdentity CreateClaims(Guid id, string role)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                    new Claim(ClaimTypes.Role, role)
                };
            ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", 
            ClaimTypes.NameIdentifier, ClaimTypes.Role);
            return claimsIdentity;
        }
    }
}