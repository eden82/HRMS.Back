using System.Text.Json.Serialization;
//using Newtonsoft.Json;


namespace HRMS.Backend.Models
{
    public class TenantSetting
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }

        //// jsonb -> nvarchar(max). Make it non-null by default.
        //public string Settings { get; set; } = "{}";

        public int Version { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool EnableSSO { get; set; } = false;
        public string SSOProvider { get; set; } = string.Empty;
        public bool RequireTwoFactorAuth { get; set; } = true;
        public int SessionTimeout { get; set; } = 60;
        public bool EnableAuditLogging { get; set; } = true;

        public bool EmailNotifications { get; set; } = true;
        public bool PushNotifications { get; set; } = false;
        public bool CriticalAlertsOnly { get; set; } = false;

        public string DefaultExportFormat { get; set; } = "CSV";
        public string BackupFrequency { get; set; } = "Daily";
        public int DataRetentionYears { get; set; } = 5;
        public bool DataEncryptionAtRest { get; set; } = true;


        [JsonIgnore]
        public Tenant Tenant { get; set; } = null!;
    }
}
