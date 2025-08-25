using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{
    
    public class Employee
    {
        [Key]
        [Column("id")]
        public int EmployeeID { get; set; }

        [Column("department_id")]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        [Column("organization_id")]
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;

        [Column("tenant_id")]
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        [Column("manager_id")]
        public int? ManagerId { get; set; }
        public Employee? Manager { get; set; }
        public ICollection<Employee> Reports { get; set; } = new List<Employee>();

        // Personal Info
        [Column("first_name"), Required, MaxLength(100)] public string FirstName { get; set; } = string.Empty;
        [Column("middle_name"), MaxLength(100)] public string? MiddleName { get; set; }
        [Column("last_name"), Required, MaxLength(100)] public string LastName { get; set; } = string.Empty;

        [Column("email"), MaxLength(255)] public string? Email { get; set; }
        [Column("phone_number"), MaxLength(50)] public string? PhoneNumber { get; set; }

        [Column("emergency_contact_name"), MaxLength(200)] public string? EmergencyContactName { get; set; }
        [Column("emergency_contact_number"), MaxLength(50)] public string? EmergencyContactNumber { get; set; }

        [Column("gender"), MaxLength(50)] public string? Gender { get; set; }
        [Column("nationality"), MaxLength(100)] public string? Nationality { get; set; }
        [Column("marital_status"), MaxLength(50)] public string? MaritalStatus { get; set; }
        [Column("address"), MaxLength(500)] public string? Address { get; set; }

        [Column("date_of_birth")] public DateTime? DateOfBirth { get; set; }

        // Job Info
        [Column("job_title"), MaxLength(150)] public string? JobTitle { get; set; }
        [Column("employee_code"), MaxLength(50)] public string? EmployeeCode { get; set; }
        [Column("employee_education_status"), MaxLength(100)] public string? EmployeeEducationStatus { get; set; }
        [Column("employee_type"), MaxLength(50)] public string? EmploymentType { get; set; }
        [Column("photo_url"), MaxLength(2083)] public string? PhotoUrl { get; set; }
        [Column("hire_date")] public DateTime? JoiningDate { get; set; }

        // Payment/Bank & flexible JSON
        [Column("bank_details")] public string? BankDetails { get; set; }
        [Column("custom_fields")] public string? CustomFields { get; set; }

        // Tracking
        [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [Column("terminated_date")] public DateTime? TerminatedDate { get; set; }

        // ===== Your legacy/extras (kept for controller compatibility) =====
        [MaxLength(100)] public string? EmergencyContact { get; set; }
        [MaxLength(10)] public string? Role { get; set; }
        [MaxLength(20)] public string? Salary { get; set; }
        [MaxLength(100)] public string? Currency { get; set; }
        [MaxLength(50)] public string? PaymentMethod { get; set; }
        [MaxLength(50)] public string? BankAccountNumber { get; set; }
        [MaxLength(50)] public string? TaxIdentificationNumber { get; set; }
        [MaxLength(50)] public string? BenefitsEnrollment { get; set; }
        [MaxLength(50)] public string? PassportNumber { get; set; }
        [MaxLength(255)] public string? ResumePath { get; set; }
        [MaxLength(255)] public string? ContractFilePath { get; set; }
        [MaxLength(255)] public string? CertificationPath { get; set; }
        [MaxLength(50)] public string? Username { get; set; }
        [MaxLength(255)] public string? PasswordHash { get; set; }
        [MaxLength(100)] public string? WorkLocation { get; set; }
        [MaxLength(255)] public string? ShiftDetails { get; set; }

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    }
}
