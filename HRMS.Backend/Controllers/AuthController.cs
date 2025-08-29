using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS.Backend.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HRMS.Backend.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || user.Password != request.Password)
                return Unauthorized(new { message = "Invalid username or password" });

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = jwtSettings["Key"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
                return StatusCode(500, "Missing JWT configuration values.");

            if (!int.TryParse(jwtSettings["ExpiryMinutes"], out int expiryMinutes))
                return StatusCode(500, "Invalid or missing ExpiryMinutes in JWT settings.");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Guid ok
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
                new Claim("organizationId", user.OrganizationId?.ToString() ?? string.Empty)
            };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            if (user.Role == "SuperAdmin")
            {
                return Ok(new
                {
                    Token = tokenString,
                    RedirectTo = "superadmin",
                    User = new { user.Id, user.Email, user.FullName, user.Role }
                });
            }
            else if (user.Role == "Admin")
            {
                return Ok(new
                {
                    Token = tokenString,
                    RedirectTo = "main",
                    User = new { user.Id, user.Email, user.FullName, user.Role, user.OrganizationId }
                });
            }

            return Ok(new
            {
                Token = tokenString,
                RedirectTo = "main",
                User = new { user.Id, user.Email, user.FullName, user.Role, user.OrganizationId }
            });
        }
    }
}
