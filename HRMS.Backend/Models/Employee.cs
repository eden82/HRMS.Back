using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{
    [Table("employees")]
    public class Employee
    {
        // ===== Keys & FKs =====
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // default NEWSEQUENTIALID set in Fluent
        public Guid EmployeeID { get; set; }

        [Required, Column("tenant_id")]
        public Guid TenantId { get; set; }

        [Required, Column("organization_id")]
        public Guid OrganizationId { get; set; }  // composite FK (Fluent)

        [Required, Column("department_id")]
        public Guid DepartmentId { get; set; }    // composite FK (Fluent)

        [Required, Column("role_id")]
        public Guid RoleId { get; set; }

        // ===== Personal =====
        [Required, MaxLength(100)]
        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Column("date_of_birth")]
        public DateTime DateOfBirth { get; set; }

        [Required, MaxLength(50)]
        [Column("gender")]
        public string Gender { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Column("nationality")]
        public string Nationality { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [Column("marital_status")]
        public string MaritalStatus { get; set; } = string.Empty;

        // ===== Contact =====
        [Required, EmailAddress, MaxLength(255)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [Column("phone_number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        [Column("address")]
        public string Address { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        [Column("emergency_contact_name")]
        public string EmergencyContactName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [Column("emergency_contact_number")]
        public string EmergencyContactNumber { get; set; } = string.Empty;

        // ===== Media / education =====
        [Required, MaxLength(2083)]
        [Column("photo_url")]
        public string PhotoUrl { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Column("employee_education_status")]
        public string EmployeeEducationStatus { get; set; } = string.Empty;

        // ===== Job =====
        [Required, MaxLength(150)]
        [Column("job_title")]
        public string JobTitle { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [Column("employee_type")]
        public string EmploymentType { get; set; } = string.Empty;

        [Required]
        [Column("hire_date")]
        public DateTime JoiningDate { get; set; }

        // Optional on input; will be auto-generated and must be unique per tenant
        [MaxLength(50)]
        [Column("employee_code")]
        public string? EmployeeCode { get; set; }

        // ===== Compensation =====
        [Required]
        [Column("salary")]
        public string Salary { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        [Column("currency")]
        public string Currency { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Column("payment_method")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Column("bank_account_number")]
        public string BankAccountNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("tax_identification_number")]
        public string? TaxIdentificationNumber { get; set; }

        [Column("bank_details")]
        public string? BankDetails { get; set; }

        // ===== Benefits & Docs =====
        [Required]
        [Column("benefits_enrollment")]
        public string BenefitsEnrollment { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("passport_number")]
        public string? PassportNumber { get; set; }

        [Required]
        [Column("resume_path")]
        public string ResumePath { get; set; } = string.Empty;

        [Column("contract_file_path")]
        public string? ContractFilePath { get; set; }

        [Column("certification_path")]
        public string? CertificationPath { get; set; }

        // ===== Access & work =====
        // Optional on input; will be auto-generated unique per tenant if missing
        [MaxLength(150)]
        [Column("username")]
        public string? Username { get; set; }

        [Required, MaxLength(150)]
        [Column("work_location")]
        public string WorkLocation { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        [Column("shift_details")]
        public string ShiftDetails { get; set; } = string.Empty;

        // ===== Extensibility & audit =====
        [Column("custom_fields")]
        public string? CustomFields { get; set; }

        [Column("created_at", TypeName = "datetime2(3)")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at", TypeName = "datetime2(3)")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("terminated_date", TypeName = "datetime2(3)")]
        public DateTime? TerminatedDate { get; set; }

        // ===== Navigations =====
        [ForeignKey(nameof(TenantId))] public Tenant Tenant { get; set; } = null!;
        public Organization Organization { get; set; } = null!; // composite (Fluent)
        public Department Department { get; set; } = null!; // composite (Fluent)
        [ForeignKey(nameof(RoleId))] public Role Role { get; set; } = null!;
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
