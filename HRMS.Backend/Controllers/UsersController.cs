using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HRMS.Backend.Data;
using HRMS.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrEmpty(request.Username))
            {
                if (request.Role != null && request.Role.Equals("superadmin", StringComparison.OrdinalIgnoreCase))
                    request.Username = "superadmin";
                else
                    return BadRequest("Username is required for non-superadmin users.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Password = request.Password, // (todo: hash)
                Role = request.Role,
                Username = request.Username,
                OrganizationId = request.OrganizationId, // null for SuperAdmin
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // GET: api/Users/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
    }

    // DTO used by UsersController (GUID)
    public sealed class CreateUserRequest
    {
        public string FullName { get; set; } = "";
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } = "";
        public string Role { get; set; } = "User";
        public string Username { get; set; } = "";
        public Guid? OrganizationId { get; set; } // null for SuperAdmin
    }
}
