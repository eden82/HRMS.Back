using System;

namespace HRMS.Backend.DTOs
{
    public class PermanentTenantSettingDto
    {
        public bool EnableSSO { get; set; } = false;
        public string SSOProvider { get; set; } = string.Empty;
        public bool RequireTwoFactorAuth { get; set; } = false;
        public string PasswordPolicy { get; set; } = "8+ chars, mixed case, numbers";
        public int SessionTimeout { get; set; } = 60;
        public bool EnableAuditLogging { get; set; } = true;

        public bool EmailNotifications { get; set; } = true;
        public bool PushNotifications { get; set; } = false;
        public bool CriticalAlertsOnly { get; set; } = false;

        public string DefaultExportFormat { get; set; } = "CSV";
        public string BackupFrequency { get; set; } = "Daily";
        public int DataRetentionYears { get; set; } = 5;
        public bool DataEncryptionAtRest { get; set; } = true;

        public DateTime? UpdatedAt { get; set; }
    }
}
