using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{
    [Table("employees")]
    public class Employee
    {
        [Key]
        [Column("id")]
        public Guid EmployeeID { get; set; } = Guid.NewGuid();

        // FK – Department / Organization / Tenant
        [Column("department_id"), Required]
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        [Column("organization_id"), Required]
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;

        [Column("tenant_id"), Required]
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        // NEW: required Role FK (no manager anymore)
        [Column("role_id"), Required]
        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;

        // Personal Info (all required)
        [Column("first_name"), Required, MaxLength(100)] public string FirstName { get; set; } = string.Empty;
        [Column("middle_name"), Required, MaxLength(100)] public string MiddleName { get; set; } = string.Empty;
        [Column("last_name"), Required, MaxLength(100)] public string LastName { get; set; } = string.Empty;

        [Column("email"), Required, MaxLength(255)] public string Email { get; set; } = string.Empty;
        [Column("phone_number"), Required, MaxLength(50)] public string PhoneNumber { get; set; } = string.Empty;

        [Column("emergency_contact_name"), Required, MaxLength(200)] public string EmergencyContactName { get; set; } = string.Empty;
        [Column("emergency_contact_number"), Required, MaxLength(50)] public string EmergencyContactNumber { get; set; } = string.Empty;

        [Column("gender"), Required, MaxLength(50)] public string Gender { get; set; } = string.Empty;
        [Column("nationality"), Required, MaxLength(100)] public string Nationality { get; set; } = string.Empty;
        [Column("marital_status"), Required, MaxLength(50)] public string MaritalStatus { get; set; } = string.Empty;
        [Column("address"), Required, MaxLength(500)] public string Address { get; set; } = string.Empty;

        [Column("date_of_birth"), Required] public DateTime DateOfBirth { get; set; }

        // Job Info (all required)
        [Column("job_title"), Required, MaxLength(150)] public string JobTitle { get; set; } = string.Empty;
        [Column("employee_code"), Required, MaxLength(50)] public string EmployeeCode { get; set; } = string.Empty;
        [Column("employee_education_status"), Required, MaxLength(100)] public string EmployeeEducationStatus { get; set; } = string.Empty;
        [Column("employee_type"), Required, MaxLength(50)] public string EmploymentType { get; set; } = string.Empty;
        [Column("photo_url"), Required, MaxLength(2083)] public string PhotoUrl { get; set; } = string.Empty;
        [Column("hire_date"), Required] public DateTime JoiningDate { get; set; }

        // Bank / Custom (required)
        [Column("bank_details"), Required] public string BankDetails { get; set; } = string.Empty;
        [Column("custom_fields"), Required] public string CustomFields { get; set; } = "{}";

        // Tracking
        [Column("created_at"), Required] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("updated_at"), Required] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [Column("terminated_date")] public DateTime? TerminatedDate { get; set; } // realistically optional

        // Navigation
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
