using Microsoft.AspNetCore.Mvc;
using HRMS.Backend.Data;
using HRMS.Backend.Models;
using Microsoft.EntityFrameworkCore;
using HRMS.Backend.DTOs;

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

        // Ensure username is provided or auto-generate for SuperAdmin
        if (string.IsNullOrEmpty(request.Username))
        {
            if (request.Role.ToLower() == "superadmin")
            {
                request.Username = "superadmin";
            }
            else
            {
                return BadRequest("Username is required for non-superadmin users.");
            }
        }

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Password = request.Password, // not hashed yet
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
            // This catches SQL errors like duplicate keys, null violations, etc.
            return StatusCode(500, $"Database error: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    // GET: api/Users/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }
}
