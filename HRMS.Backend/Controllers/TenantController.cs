using Microsoft.AspNetCore.Mvc;
using HRMS.Backend.Models;
using HRMS.Backend.Data;
using Microsoft.EntityFrameworkCore;




[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TenantsController(AppDbContext context)
    {
        _context = context;
    }

    // POST: api/Tenants
    [HttpPost]
    public async Task<IActionResult> CreateTenant([FromBody] Tenant tenant)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTenantById), new { id = tenant.Id }, tenant);
    }

    // GET: api/Tenants/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTenantById(int id)
    {
        var tenant = await _context.Tenants.FindAsync(id);
        if (tenant == null)
            return NotFound();

        return Ok(tenant);
    }

    // DELETE: api/Tenants/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTenant(int id)
    {
        var tenant = await _context.Tenants
            .Include(t => t.Organizations)
            .Include(t => t.Roles)
            .Include(t => t.Users)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tenant == null)
            return NotFound();

        // Manual deletion because of cascade restriction
        _context.Users.RemoveRange(tenant.Users);
        _context.Roles.RemoveRange(tenant.Roles);
        _context.Organizations.RemoveRange(tenant.Organizations);
        _context.Tenants.Remove(tenant);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
