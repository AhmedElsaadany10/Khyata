using khyata.Application.Interfaces.Services;
using khyata.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace khyata.Infrastructure.Services
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
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claim("wid",user.WorkspaceId.ToString()),
                new Claim(ClaimTypes.Role,user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            };
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(ExpiresInSeconds),
                signingCredentials: creds
                );
            return _jwtHandler.WriteToken(token);
        }
    }
}
