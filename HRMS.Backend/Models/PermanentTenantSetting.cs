using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Backend.Models
{
    public class PermanentTenantSetting
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Authentication and security
        public bool EnableSSO { get; set; } = false;
        public string SSOProvider { get; set; } = string.Empty;
        public bool RequireTwoFactorAuth { get; set; } = true;
        public int SessionTimeout { get; set; } = 60;
        public bool EnableAuditLogging { get; set; } = true;

        // Notifications
        public bool EmailNotifications { get; set; } = true;
        public bool PushNotifications { get; set; } = false;
        public bool CriticalAlertsOnly { get; set; } = false;

        // Data & backup
        public string DefaultExportFormat { get; set; } = "CSV";
        public string BackupFrequency { get; set; } = "Daily";
        public int DataRetentionYears { get; set; } = 5;
        public bool DataEncryptionAtRest { get; set; } = true;

        // Audit info
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
