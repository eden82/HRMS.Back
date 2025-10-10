using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS.Backend.Data;
using HRMS.Backend.DTOs;
using HRMS.Backend.Models;
using HRMS.Backend.Filters;

namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RoleAuthorize("SuperAdmin , SystemAdmin")]
    public class TenantSettingsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TenantSettingsController(AppDbContext db)
        {
            _db = db;
        }


        [HttpPost]
        public async Task<IActionResult> CreateTenantSetting([FromBody] CreateTenantSettingDto input)
        {
            if (input == null) return BadRequest("Request body is required.");

            // Check if setting for this tenant already exists
            var existing = await _db.TenantSettings.FirstOrDefaultAsync(s => s.TenantId == input.TenantId);
            if (existing != null)
                return Conflict("Tenant setting for this tenant already exists.");

            var setting = new TenantSetting
            {
                TenantId = input.TenantId,
                EnableSSO = input.EnableSSO,
                SSOProvider = input.SSOProvider,
                RequireTwoFactorAuth = input.RequireTwoFactorAuth,
                PasswordPolicy = input.PasswordPolicy,
                SessionTimeout = input.SessionTimeout,
                EnableAuditLogging = input.EnableAuditLogging,

                EmailNotifications = input.EmailNotifications,
                PushNotifications = input.PushNotifications,
                CriticalAlertsOnly = input.CriticalAlertsOnly,

                DefaultExportFormat = input.DefaultExportFormat,
                BackupFrequency = input.BackupFrequency,
                DataRetentionYears = input.DataRetentionYears,
                DataEncryptionAtRest = input.DataEncryptionAtRest,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.TenantSettings.Add(setting);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(UpdateTenantSetting), new { tenantId = setting.TenantId }, setting);
        }


        [HttpPut("{tenantId:guid}")]
        public async Task<IActionResult> UpdateTenantSetting(Guid tenantId, [FromBody] CreateTenantSettingDto input)
        {
            var setting = await _db.TenantSettings.FirstOrDefaultAsync(s => s.TenantId == tenantId);
            if (setting == null) return NotFound("Tenant setting not found.");

            // Update values
            setting.EnableSSO = input.EnableSSO;
            setting.SSOProvider = input.SSOProvider;
            setting.RequireTwoFactorAuth = input.RequireTwoFactorAuth;
            setting.PasswordPolicy = input.PasswordPolicy;
            setting.SessionTimeout = input.SessionTimeout;
            setting.EnableAuditLogging = input.EnableAuditLogging;

            setting.EmailNotifications = input.EmailNotifications;
            setting.PushNotifications = input.PushNotifications;
            setting.CriticalAlertsOnly = input.CriticalAlertsOnly;

            setting.DefaultExportFormat = input.DefaultExportFormat;
            setting.BackupFrequency = input.BackupFrequency;
            setting.DataRetentionYears = input.DataRetentionYears;
            setting.DataEncryptionAtRest = input.DataEncryptionAtRest;
            setting.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok(setting);
        }
    }
}
