// Controllers/TenantsController.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HRMS.Backend.Models;
using HRMS.Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using HRMS.Backend.Filters;


namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RoleAuthorize("SuperAdmin")]
    public class TenantsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TenantsController(AppDbContext context) => _context = context;

        [HttpPost]
        public async Task<IActionResult> CreateTenant([FromBody] Tenant tenant)
        {
            if (string.IsNullOrWhiteSpace(tenant.Domain))
                ModelState.AddModelError(nameof(tenant.Domain), "Domain is required.");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (tenant.Id == Guid.Empty)
                tenant.Id = Guid.NewGuid();

            // Save tenant first
            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            //  Declare tenantSetting outside to keep scope valid
            TenantSetting? tenantSetting = null;

            // Fetch permanent default setting
            var permanentSetting = await _context.PermanentTenantSettings.AsNoTracking().FirstOrDefaultAsync();

            if (permanentSetting != null)
            {
                tenantSetting = new TenantSetting
                {
                    TenantId = tenant.Id,
                    EnableSSO = permanentSetting.EnableSSO,
                    SSOProvider = permanentSetting.SSOProvider,
                    RequireTwoFactorAuth = permanentSetting.RequireTwoFactorAuth,
                    PasswordPolicy = permanentSetting.PasswordPolicy,
                    SessionTimeout = permanentSetting.SessionTimeout,
                    EnableAuditLogging = permanentSetting.EnableAuditLogging,

                    EmailNotifications = permanentSetting.EmailNotifications,
                    PushNotifications = permanentSetting.PushNotifications,
                    CriticalAlertsOnly = permanentSetting.CriticalAlertsOnly,

                    DefaultExportFormat = permanentSetting.DefaultExportFormat,
                    BackupFrequency = permanentSetting.BackupFrequency,
                    DataRetentionYears = permanentSetting.DataRetentionYears,
                    DataEncryptionAtRest = permanentSetting.DataEncryptionAtRest,

                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.TenantSettings.Add(tenantSetting);
                await _context.SaveChangesAsync();
            }

            // Return both even if tenantSetting = null
            return CreatedAtAction(nameof(GetTenantById), new { id = tenant.Id }, new
            {
                Tenant = tenant,
                TenantSetting = tenantSetting
            });
        }




        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTenantById(Guid id)
        {
            var tenant = await _context.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (tenant == null) return NotFound();
            return Ok(tenant);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tenant>>> GetAll() =>
            Ok(await _context.Tenants.AsNoTracking().ToListAsync());

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Tenant body)
        {
            var t = await _context.Tenants.FirstOrDefaultAsync(x => x.Id == id);
            if (t is null) return NotFound();

            if (string.IsNullOrWhiteSpace(body.Domain))
                return BadRequest(new { message = "Domain is required." });

            t.Name = body.Name?.Trim() ?? t.Name;
            t.Domain = body.Domain.Trim(); // required
            t.Industry = body.Industry;
            t.Location = body.Location;
            t.Country = body.Country;
            t.TimeZone = body.TimeZone;
            t.Description = body.Description;
            t.EmployeeManagement = body.EmployeeManagement;
            t.AttendanceTracking = body.AttendanceTracking;
            t.LeaveManagement = body.LeaveManagement;
            t.Recruitment = body.Recruitment;
            t.PerformanceManagement = body.PerformanceManagement;
            t.TrainingDevelopment = body.TrainingDevelopment;

            t.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTenant(Guid id)
        {
            var tenant = await _context.Tenants
                .Include(t => t.Organizations)
                .Include(t => t.Roles)
                .Include(t => t.Users)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tenant == null) return NotFound();

            _context.Users.RemoveRange(tenant.Users);
            _context.Roles.RemoveRange(tenant.Roles);
            _context.Organizations.RemoveRange(tenant.Organizations);
            _context.Tenants.Remove(tenant);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
