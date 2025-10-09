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
