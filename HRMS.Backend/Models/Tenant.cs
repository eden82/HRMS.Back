// Models/Tenant.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{
    [Table("tenants")]
    public class Tenant
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("tenant_name"), Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // NOW REQUIRED
        [Column("domain"), Required, MaxLength(255)]
        public string Domain { get; set; } = string.Empty;

        [Column("industry"), MaxLength(100)]
        public string? Industry { get; set; }

        [Column("location"), MaxLength(200)]
        public string? Location { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string Country { get; set; } = string.Empty;
        public string TimeZone { get; set; } = string.Empty;

        public bool EmployeeManagement { get; set; } = true;
        public bool AttendanceTracking { get; set; } = false;
        public bool LeaveManagement { get; set; } = false;
        public bool Recruitment { get; set; } = false;
        public bool PerformanceManagement { get; set; } = false;
        public bool TrainingDevelopment { get; set; } = false;

        [MaxLength(50)]
        public string Status { get; set; } = "Active";

        // Navigations
        public ICollection<Organization> Organizations { get; set; } = new List<Organization>();
        public ICollection<Department> Departments { get; set; } = new List<Department>();
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();

    }
}
