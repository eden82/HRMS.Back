using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS.Backend.Data;
using HRMS.Backend.Models;
using HRMS.Backend.DTOs;
using HRMS.Backend.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/organizations")]
    [Produces("application/json")]
    [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
    public class OrganizationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public OrganizationsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }



        // GET: /api/organizations/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrganizationDto>> GetById(Guid id)
        {
            var org = await _context.Organizations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (org == null) return NotFound();

            return Ok(new OrganizationDto
            {
                Id = org.Id,
                TenantId = org.TenantId,
                Name = org.Name ?? string.Empty,
                Domain = org.Domain ?? string.Empty,
                Industry = org.Industry ?? string.Empty,
                Location = org.Location ?? string.Empty,
                LogoUrl = org.LogoUrl ?? string.Empty,
                OrgCode = org.OrgCode ?? string.Empty,
                Description = org.Description ?? string.Empty,
                IpRestrictions = org.IpRestrictions
            });

        }
        // GET: /api/organizations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrganizationDto>>> GetAll()
        {
            var orgs = await _context.Organizations
                .AsNoTracking()
                .Select(org => new OrganizationDto
                {
                    Id = org.Id,
                    TenantId = org.TenantId,
                    Name = org.Name ?? string.Empty,
                    Domain = org.Domain ?? string.Empty,
                    Industry = org.Industry ?? string.Empty,
                    Location = org.Location ?? string.Empty,
                    LogoUrl = org.LogoUrl ?? string.Empty,
                    OrgCode = org.OrgCode ?? string.Empty,
                    Description = org.Description ?? string.Empty,
                    IpRestrictions = org.IpRestrictions
                })
                .ToListAsync();

            return Ok(orgs);
        }
        // GET: /api/organizations
        [HttpGet("by-tenant/{tenantId:guid}")]
        public async Task<ActionResult<IEnumerable<OrganizationDto>>> GetByTenantId(Guid tenantId)
        {
            var orgs = await _context.Organizations
                .AsNoTracking()
                .Where(org => org.TenantId == tenantId) // filter by tenant
                .Select(org => new OrganizationDto
                {
                    Id = org.Id,
                    TenantId = org.TenantId,
                    Name = org.Name ?? string.Empty,
                    Domain = org.Domain ?? string.Empty,
                    Industry = org.Industry ?? string.Empty,
                    Location = org.Location ?? string.Empty,
                    LogoUrl = org.LogoUrl ?? string.Empty,
                    OrgCode = org.OrgCode ?? string.Empty,
                    Description = org.Description ?? string.Empty,
                    IpRestrictions = org.IpRestrictions
                })
                .ToListAsync();

            return Ok(orgs);
        }



        // POST: /api/organizations
        [HttpPost]
        [RoleAuthorize("SuperAdmin , SystemAdmin")]
        public async Task<ActionResult<OrganizationDto>> Create([FromForm] CreateOrganizationDto input)
        {
            // Treat whitespace as empty
            if (string.IsNullOrWhiteSpace(input.Domain))
                ModelState.AddModelError(nameof(input.Domain), "Domain can't be empty");
            if (string.IsNullOrWhiteSpace(input.Industry))
                ModelState.AddModelError(nameof(input.Industry), "Industry can't be empty");
            if (string.IsNullOrWhiteSpace(input.Location))
                ModelState.AddModelError(nameof(input.Location), "Location can't be empty");
            if (input.LogoUrl == null)
                ModelState.AddModelError(nameof(input.LogoUrl), "Logo file is required");


            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var tenantExists = await _context.Tenants.AnyAsync(t => t.Id == input.TenantId);
            if (!tenantExists) return BadRequest($"Tenant {input.TenantId} not found.");


            // Check if domain already exists in organizations for this tenant
            var domainExists = await _context.Organizations
                .AnyAsync(o => o.TenantId == input.TenantId &&
                               o.Domain.ToUpper() == input.Domain.Trim().ToUpper());

            if (domainExists)
                return BadRequest(new { message = $"The domain '{input.Domain}' is already in use for this tenant." });




            // OrgCode: provided (normalized) or auto-generate unique per tenant
            var orgCode = string.IsNullOrWhiteSpace(input.OrgCode)
                ? await GenerateUniqueOrgCodeAsync(input.TenantId, input.Name)
                : input.OrgCode!.Trim().ToUpperInvariant();

            // If client supplied a code, ensure unique within tenant
            if (!string.IsNullOrWhiteSpace(input.OrgCode))
            {
                var clash = await _context.Organizations
                    .AnyAsync(o => o.TenantId == input.TenantId && o.OrgCode == orgCode);
                if (clash) return Conflict(new { message = $"org_code '{orgCode}' already exists for this tenant." });
            }

            string? logourl = null;

            // logo
            if (input.LogoUrl != null)
            {

                var rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadsFolderLogo = Path.Combine(rootPath, "uploads", "Organization", "Logo");


                if (!Directory.Exists(uploadsFolderLogo)) Directory.CreateDirectory(uploadsFolderLogo);

                var fileNameLogo = Guid.NewGuid().ToString() + Path.GetExtension(input.LogoUrl.FileName);
                var filePathLogo = Path.Combine(uploadsFolderLogo, fileNameLogo);

                using (var stream = new FileStream(filePathLogo, FileMode.Create))
                    await input.LogoUrl.CopyToAsync(stream);

                logourl = $"/uploads/Organization/{fileNameLogo}";
            }

            var org = new Organization
            {
                Id = Guid.NewGuid(),
                TenantId = input.TenantId,
                Name = input.Name.Trim(),
                Domain = input.Domain.Trim(),   // REQUIRED
                Industry = input.Industry.Trim(),
                Location = input.Location.Trim(),
                //LogoUrl = input.LogoUrl.Trim(),
                LogoUrl = logourl ?? string.Empty,
                OrgCode = orgCode,
                IpRestrictions = string.IsNullOrWhiteSpace(input.IpRestrictions) ? null : input.IpRestrictions!.Trim(),
                Description = input.Description?.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Organizations.Add(org);
            await _context.SaveChangesAsync();

            var dto = new OrganizationDto
            {
                Id = org.Id,
                TenantId = org.TenantId,
                Name = org.Name,
                Domain = org.Domain,
                Industry = org.Industry,
                Location = org.Location,
                OrgCode = org.OrgCode ?? string.Empty,
                LogoUrl = org.LogoUrl ?? string.Empty,
                IpRestrictions = org.IpRestrictions
            };



            return CreatedAtAction(nameof(GetById), new { id = org.Id }, dto);
        }

        // PUT: /api/organizations/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateOrganizationDto input)
        {
            if (id != input.Id) return BadRequest("Organization ID mismatch.");

            if (string.IsNullOrWhiteSpace(input.Domain))
                ModelState.AddModelError(nameof(input.Domain), "Domain can't be empty");
            if (string.IsNullOrWhiteSpace(input.Industry))
                ModelState.AddModelError(nameof(input.Industry), "Industry can't be empty");
            if (string.IsNullOrWhiteSpace(input.Location))
                ModelState.AddModelError(nameof(input.Location), "Location can't be empty");
            if (input.LogoUrl == null)
                ModelState.AddModelError(nameof(input.LogoUrl), "Logo file is required");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);


            var org = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == id);
            if (org == null) return NotFound();

            if (org.TenantId != input.TenantId)
            {
                var tenantExists = await _context.Tenants.AnyAsync(t => t.Id == input.TenantId);
                if (!tenantExists) return BadRequest($"Tenant {input.TenantId} not found.");
                org.TenantId = input.TenantId;
            }


            //  Handle file upload (replace old logo if a new one is uploaded)
            if (input.LogoUrl != null && input.LogoUrl.Length > 0)
            {
                var rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadsFolder = Path.Combine(rootPath, "uploads", "Organization", "Logo");

                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                // Optional: delete old logo file
                if (!string.IsNullOrWhiteSpace(org.LogoUrl))
                {
                    var oldFilePath = Path.Combine(rootPath, org.LogoUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Save new logo
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(input.LogoUrl.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await input.LogoUrl.CopyToAsync(stream);

                org.LogoUrl = $"/uploads/Organization/Logo/{fileName}";
            }

            org.Name = input.Name.Trim();
            org.Domain = input.Domain.Trim();   // REQUIRED
            org.Industry = input.Industry.Trim();
            org.Location = input.Location.Trim();


            org.Description = string.IsNullOrWhiteSpace(input.Description)
                ? null
                : input.Description!.Trim();

            if (!string.IsNullOrWhiteSpace(input.OrgCode))
            {
                var newCode = input.OrgCode.Trim().ToUpperInvariant();
                if (!string.Equals(newCode, org.OrgCode, StringComparison.Ordinal))
                {
                    var clash = await _context.Organizations
                        .AnyAsync(o => o.TenantId == org.TenantId && o.OrgCode == newCode && o.Id != org.Id);
                    if (clash) return Conflict(new { message = $"org_code '{newCode}' already exists for this tenant." });
                    org.OrgCode = newCode;
                }
            }

            org.IpRestrictions = string.IsNullOrWhiteSpace(input.IpRestrictions)
                ? null
                : input.IpRestrictions!.Trim();

            org.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: /api/organizations/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var org = await _context.Organizations
                .Include(o => o.Departments)
                .Include(o => o.Employees)
                .Include(o => o.LeaveTypes)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (org == null) return NotFound();

            if (org.LeaveTypes?.Count > 0) _context.LeaveTypes.RemoveRange(org.LeaveTypes);
            if (org.Employees?.Count > 0) _context.Employees.RemoveRange(org.Employees);
            if (org.Departments?.Count > 0) _context.Departments.RemoveRange(org.Departments);

            _context.Organizations.Remove(org);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ===== Helpers =====
        private async Task<string> GenerateUniqueOrgCodeAsync(Guid tenantId, string name)
        {
            // Base from name: letters/digits only, upper, 3–6 chars
            var raw = new string((name ?? "ORG").Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
            if (raw.Length < 3) raw = (raw + "XXX").Substring(0, 3);
            var baseCode = raw.Substring(0, Math.Min(6, raw.Length));

            // Try random suffix until unique
            string candidate;
            do
            {
                var suffix = RandomNumberGenerator.GetInt32(100, 1000); // 100-999
                candidate = $"{baseCode}-{suffix}";
            }
            while (await _context.Organizations.AnyAsync(o => o.TenantId == tenantId && o.OrgCode == candidate));

            return candidate;
        }
    }
}
