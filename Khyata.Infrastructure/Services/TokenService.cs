using Khyata.Application.Interfaces.IServices;
using Khyata.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;

namespace Khyata.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        private readonly JwtSecurityTokenHandler _jwtHandler;
        public TokenService(IConfiguration config)
        {
            _config = config;
            _jwtHandler = new JwtSecurityTokenHandler();
        }
        public int ExpiresInSeconds =>
           int.Parse(_config["Jwt:ExpiresInMinutes"] ?? "60") * 60;

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
            new Claim("wid",  user.WorkspaceId.ToString()),
            new Claim("workspace_role", user.Role.ToString()),
            new Claim("type", "workspace"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
            return BuildToken(claims);
        }
        // Admin-scoped JWT (different audience = cannot be used on main API)
        public string GenerateAdminToken(AdminUser admin, IList<string> roles)
        {
            var claims = new List<Claim>
            {
            new Claim(JwtRegisteredClaimNames.Sub, admin.Id.ToString()),
            new(JwtRegisteredClaimNames.Name, admin.UserName!),
            new Claim("type", "admin"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
            // Add all role claims — supports multi-role (SuperAdmin + Admin + Moderator)
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
            return BuildToken(claims.ToList());
        }
        private string BuildToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(ExpiresInSeconds),
                signingCredentials: creds);
            return _jwtHandler.WriteToken(token);
        }
    }
}
