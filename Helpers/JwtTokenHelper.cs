using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Vikalp.Helpers
{
    public sealed class JwtTokenHelper
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly double _expireMinutes;

        public JwtTokenHelper(IConfiguration config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            _key = config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");
            _issuer = config["Jwt:Issuer"] ?? string.Empty;
            _audience = config["Jwt:Audience"] ?? string.Empty;

            if (!double.TryParse(config["Jwt:ExpireMinutes"], out _expireMinutes))
            {
                _expireMinutes = 60 * 8; // default 8 hours
            }
        }

        public string GenerateToken(Guid userId, string username, int roleId)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username ?? string.Empty),
                new Claim(ClaimTypes.Role, roleId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_expireMinutes);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
