using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Backend.Models
{
    public class Organization
    {
        public int Id { get; set; }

        // ===== Organization Details =====
        public string Name { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string CompanySize { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // ===== Administrator Details =====
        public string AdminFirstName { get; set; } = string.Empty;
        public string AdminLastName { get; set; } = string.Empty;
        public string AdminEmail { get; set; } = string.Empty;
        public string AdminPhone { get; set; } = string.Empty;

        // ===== Regional Settings =====
        public string Country { get; set; } = string.Empty;
        public string TimeZone { get; set; } = string.Empty;

        // ===== Modules =====
        public bool EmployeeManagement { get; set; } = true;
        public bool AttendanceTracking { get; set; } = false;
        public bool LeaveManagement { get; set; } = false;
        public bool Recruitment { get; set; } = false;
        public bool PerformanceManagement { get; set; } = false;
        public bool TrainingDevelopment { get; set; } = false;

        // ===== Security Settings =====
        public bool EnableSSO { get; set; } = false;
        public string SSOProvider { get; set; } = string.Empty;
        public string? IpRestrictions { get; set; }
        public bool RequireTwoFactorAuth { get; set; } = false;
        public string PasswordPolicy { get; set; } = "8+ chars, mixed case, numbers";
        public int SessionTimeout { get; set; } = 60;
        public bool EnableAuditLogging { get; set; } = true;

        // ===== Notification Settings =====
        public bool EmailNotifications { get; set; } = true;
        public bool PushNotifications { get; set; } = false;
        public bool CriticalAlertsOnly { get; set; } = false;

        // ===== Data & Backup =====
        public string BackupFrequency { get; set; } = "Daily";
        public int DataRetentionYears { get; set; } = 5;
        public string DefaultExportFormat { get; set; } = "CSV";
        public bool DataEncryptionAtRest { get; set; } = true;

        // ===== Timestamps =====
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // ===== Relationships =====

        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;
        public ICollection<Department> Departments { get; set; } = new List<Department>();

    }
}

