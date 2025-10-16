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
    [RoleAuthorize("SuperAdmin")]
    public class PermanentTenantSettingController : ControllerBase
    {
        private readonly AppDbContext _db;
        public PermanentTenantSettingController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var setting = await _db.PermanentTenantSettings.AsNoTracking().FirstOrDefaultAsync();
            if (setting == null)
                return NotFound("No permanent settings found.");

            return Ok(setting);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate([FromBody] PermanentTenantSettingDto input)
        {
            var existing = await _db.PermanentTenantSettings.FirstOrDefaultAsync();

            if (existing == null)
            {
                var newSetting = new PermanentTenantSetting
                {
                    EnableSSO = input.EnableSSO,
                    SSOProvider = input.SSOProvider,
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
                };
                _db.PermanentTenantSettings.Add(newSetting);
            }
            else
            {
                existing.EnableSSO = input.EnableSSO;
                existing.SSOProvider = input.SSOProvider;
                existing.SessionTimeout = input.SessionTimeout;
                existing.EnableAuditLogging = input.EnableAuditLogging;
                existing.EmailNotifications = input.EmailNotifications;
                existing.PushNotifications = input.PushNotifications;
                existing.CriticalAlertsOnly = input.CriticalAlertsOnly;
                existing.DefaultExportFormat = input.DefaultExportFormat;
                existing.BackupFrequency = input.BackupFrequency;
                existing.DataRetentionYears = input.DataRetentionYears;
                existing.DataEncryptionAtRest = input.DataEncryptionAtRest;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            return Ok(await _db.PermanentTenantSettings.FirstOrDefaultAsync());
        }
    }
}
