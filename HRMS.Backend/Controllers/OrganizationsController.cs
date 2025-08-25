using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS.Backend.Data;
using HRMS.Backend.Models;

namespace HRMS.Backend.Controllers
{
	[ApiController]
	[Route("api/organizations")]
	[Route("api/organization")] // alias to avoid route typos -> both work
	public class OrganizationsController : ControllerBase
	{
		private readonly AppDbContext _context;
		public OrganizationsController(AppDbContext context) => _context = context;

		// GET: /api/organizations
		[HttpGet]
		public async Task<ActionResult<IEnumerable<OrganizationDto>>> GetAll()
		{
			var orgs = await _context.Organizations
				.AsNoTracking()
				.Select(o => new OrganizationDto(o.Id, o.TenantId, o.Name, o.Domain))
				.ToListAsync();

			return Ok(orgs); // returns [] if none (never 404 on collection)
		}

		// GET: /api/organizations/5
		[HttpGet("{id:int}")]
		public async Task<ActionResult<OrganizationDto>> GetById(int id)
		{
			var o = await _context.Organizations
				.AsNoTracking()
				.FirstOrDefaultAsync(x => x.Id == id);

			if (o == null) return NotFound();
			return Ok(new OrganizationDto(o.Id, o.TenantId, o.Name, o.Domain));
		}

		// POST: /api/organizations
		[HttpPost]
		public async Task<ActionResult<OrganizationDto>> Create(CreateOrganizationDto input)
		{
			// basic guard: tenant must exist
			var tenantExists = await _context.Tenants.AnyAsync(t => t.Id == input.TenantId);
			if (!tenantExists) return BadRequest($"Tenant {input.TenantId} not found.");

			var org = new Organization
			{
				TenantId = input.TenantId,
				Name = input.Name,
				Domain = input.Domain
			};

			_context.Organizations.Add(org);
			await _context.SaveChangesAsync();

			var dto = new OrganizationDto(org.Id, org.TenantId, org.Name, org.Domain);
			return CreatedAtAction(nameof(GetById), new { id = org.Id }, dto);
		}

		// PUT: /api/organizations/5
		[HttpPut("{id:int}")]
		public async Task<IActionResult> Update(int id, UpdateOrganizationDto input)
		{
			if (id != input.Id) return BadRequest("Organization ID mismatch.");

			var org = await _context.Organizations.FindAsync(id);
			if (org == null) return NotFound();

			if (org.TenantId != input.TenantId)
			{
				var tenantExists = await _context.Tenants.AnyAsync(t => t.Id == input.TenantId);
				if (!tenantExists) return BadRequest($"Tenant {input.TenantId} not found.");
				org.TenantId = input.TenantId;
			}

			org.Name = input.Name;
			org.Domain = input.Domain;

			await _context.SaveChangesAsync();
			return NoContent();
		}

		// DELETE: /api/organizations/5
		[HttpDelete("{id:int}")]
		public async Task<IActionResult> Delete(int id)
		{
			var org = await _context.Organizations.FindAsync(id);
			if (org == null) return NotFound();

			_context.Organizations.Remove(org);
			await _context.SaveChangesAsync();
			return NoContent();
		}
	}

	// DTOs (compact, no nullability warnings)
	public record OrganizationDto(int Id, int TenantId, string Name, string? Domain);
	public record CreateOrganizationDto(int TenantId, string Name, string? Domain);
	public record UpdateOrganizationDto(int Id, int TenantId, string Name, string? Domain);
}
