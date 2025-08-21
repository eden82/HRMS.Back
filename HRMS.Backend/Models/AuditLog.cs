
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int TenantId { get; set; }
        public string Action { get; set; }
        public string Module { get; set; }
        public DateTime Timestamp { get; set; }
        public string IpAddress { get; set; }

        // JSONB stored as string
        public string Details { get; set; }

        // Navigation
        public Employee Employee { get; set; }
        public Tenant Tenant { get; set; }
    }
}