using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HRMS.Backend.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HRMS.Backend.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config) => _config = config;

        public Task<(string Jwt, DateTimeOffset ExpiresAt, string Jti)> CreateAccessTokenAsync(User user)
        {
            var key = _config["JwtSettings:Key"] ?? throw new InvalidOperationException("JwtSettings:Key missing");
            var iss = _config["JwtSettings:Issuer"];
            var aud = _config["JwtSettings:Audience"];
            var mins = int.TryParse(_config["JwtSettings:AccessTokenMinutes"], out var m) ? m : 60;

            var jti = Guid.NewGuid().ToString("N");
            var now = DateTimeOffset.UtcNow;
            var exp = now.AddMinutes(mins);

            var claimsList = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("security_stamp", user.SecurityStamp ?? string.Empty),
                new Claim("tenant_id", user.TenantId?.ToString() ?? string.Empty),
                new Claim("org_id", user.OrganizationId?.ToString() ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            var roleNames = user.UserRoles?.Select(ur => ur.Role?.Name).Where(r => !string.IsNullOrEmpty(r)).ToList() ?? new List<string>();
            foreach (var role in roleNames)
                claimsList.Add(new Claim(ClaimTypes.Role, role));

            var permissions = user.UserRoles?
                .SelectMany(ur => System.Text.Json.JsonSerializer.Deserialize<List<string>>(ur.Role?.PermissionsJson ?? "[]") ?? new List<string>())
                .Distinct()
                .ToList() ?? new List<string>();

            foreach (var perm in permissions)
                claimsList.Add(new Claim("permission", perm));

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: iss,
                audience: aud,
                claims: claimsList,
                notBefore: now.UtcDateTime,
                expires: exp.UtcDateTime,
                signingCredentials: credentials
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Task.FromResult((jwt, exp, jti));
        }

        public (string RefreshToken, DateTimeOffset ExpiresAt) CreateRefreshToken()
        {
            var mins = int.TryParse(_config["JwtSettings:RefreshTokenMinutes"], out var m) ? m : 7 * 24 * 60;
            var exp = DateTimeOffset.UtcNow.AddMinutes(mins);

            var bytes = RandomNumberGenerator.GetBytes(32);
            var token = Convert.ToBase64String(bytes);

            return (token, exp);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var key = _config["JwtSettings:Key"];
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateLifetime = false // ignore expiration
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken)
                    return null;
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
