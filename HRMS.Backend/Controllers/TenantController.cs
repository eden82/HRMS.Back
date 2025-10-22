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
using HRMS.Backend.DTOs;


namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RoleAuthorize("SuperAdmin, SystemAdmin,HR")]
    public class TenantsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TenantsController(AppDbContext context) => _context = context;

        [HttpPost]
        public async Task<IActionResult> CreateTenant([FromBody] Tenant tenant)
        {
            if (string.IsNullOrWhiteSpace(tenant.Domain))
                ModelState.AddModelError(nameof(tenant.Domain), "Domain is required.");


            // Validate domain format using Regex
            var domainPattern = @"^(?:[a-zA-Z0-9](?:[a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}$";
            if (!string.IsNullOrWhiteSpace(tenant.Domain) && !System.Text.RegularExpressions.Regex.IsMatch(tenant.Domain, domainPattern))
            {
                ModelState.AddModelError(nameof(tenant.Domain), "Invalid domain format. Example: example.com");
            }

            // Ensure no duplicate domain
            var exists = await _context.Tenants.AnyAsync(t => t.Domain == tenant.Domain);
            if (exists)
                ModelState.AddModelError(nameof(tenant.Domain), "This domain already exists.");


            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);


            //  Check if PermanentTenantSettings exist
            var permanentSetting = await _context.PermanentTenantSettings.AsNoTracking().FirstOrDefaultAsync();
            if (permanentSetting == null)
            {
                return BadRequest(new { message = "Please create a permanent tenant setting first before adding a new tenant." });
            }


            if (tenant.Id == Guid.Empty)
                tenant.Id = Guid.NewGuid();

            // Save tenant first
            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            //  Declare tenantSetting outside to keep scope valid
            TenantSetting? tenantSetting = null;



            if (permanentSetting != null)
            {

                // --- Validation: SSOProvider must be null if EnableSSO is false ---
                if (!permanentSetting.EnableSSO && !string.IsNullOrWhiteSpace(permanentSetting.SSOProvider))
                {
                    return BadRequest(new { message = "PermanentTenantSetting has EnableSSO = false but SSOProvider is not null. Please fix it first." });
                }


                tenantSetting = new TenantSetting
                {
                    TenantId = tenant.Id,
                    EnableSSO = permanentSetting.EnableSSO,
                    SSOProvider = permanentSetting.SSOProvider,
                    SessionTimeout = permanentSetting.SessionTimeout,
                    EnableAuditLogging = permanentSetting.EnableAuditLogging,

                    EmailNotifications = permanentSetting.EmailNotifications,
                    PushNotifications = permanentSetting.PushNotifications,
                    CriticalAlertsOnly = permanentSetting.CriticalAlertsOnly,

                    DefaultExportFormat = permanentSetting.DefaultExportFormat,
                    BackupFrequency = permanentSetting.BackupFrequency,
                    DataRetentionYears = permanentSetting.DataRetentionYears,
                    DataEncryptionAtRest = permanentSetting.DataEncryptionAtRest,
                    RequireTwoFactorAuth = permanentSetting.RequireTwoFactorAuth,

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
        public async Task<ActionResult<IEnumerable<TenantDto>>> GetAll()
        {
            var tenants = await _context.Tenants
                .AsNoTracking()
                .Select(t => new TenantDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Domain = t.Domain,
                    Status = t.Status,       
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    Industry = t.Industry,
                    Location = t.Location,
                    Description = t.Description,
                    Country = t.Country,
                    TimeZone = t.TimeZone,
                    EmployeeManagement = t.EmployeeManagement,
                    AttendanceTracking = t.AttendanceTracking,
                    LeaveManagement = t.LeaveManagement,
                    Recruitment = t.Recruitment,
                    PerformanceManagement = t.PerformanceManagement,
                    TrainingDevelopment = t.TrainingDevelopment
                })
                .ToListAsync();

            return Ok(tenants);
        }



        [HttpGet("{tenantId:guid}/modules")]
        [RoleAuthorize("SuperAdmin,SystemAdmin,HR")]
        public async Task<ActionResult<TenantModulesDto>> GetTenantModules(Guid tenantId)
        {
            var t = await _context.Tenants.AsNoTracking().FirstOrDefaultAsync(x => x.Id == tenantId);
            if (t == null) return NotFound();

            return Ok(new TenantModulesDto
            {
                EmployeeManagement = t.EmployeeManagement,
                AttendanceTracking = t.AttendanceTracking,
                LeaveManagement = t.LeaveManagement,
                Recruitment = t.Recruitment,
                PerformanceManagement = t.PerformanceManagement,
                TrainingDevelopment = t.TrainingDevelopment
            });
        }



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
