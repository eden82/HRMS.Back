using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Backend.DTOs
{
    // Read DTO (what you return to clients)
    public class EmployeeDto
    {
        public Guid Id { get; set; }                 // maps from Employee.EmployeeID
        public Guid DepartmentId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid TenantId { get; set; }
        public Guid RoleId { get; set; }

        // Personal & contact
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // Job
        public DateTime DateOfBirth { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeEducationStatus { get; set; } = string.Empty;
        public string EmploymentType { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public DateTime JoiningDate { get; set; }

        // Other
        public string BankDetails { get; set; } = string.Empty;
        public string CustomFields { get; set; } = "{}";

        // Tracking
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? TerminatedDate { get; set; }
    }

    // Write DTO (what clients send on create/update)
    public class EmployeeCreateUpdateDto
    {
        // Required FKs
        [Required] public Guid DepartmentId { get; set; } // org/tenant derived from this
        [Required] public Guid RoleId { get; set; }

        // Required personal & contact
        [Required, MaxLength(100)] public string FirstName { get; set; } = string.Empty;
        [Required, MaxLength(100)] public string MiddleName { get; set; } = string.Empty;
        [Required, MaxLength(100)] public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(255)] public string Email { get; set; } = string.Empty;
        [Required, MaxLength(50)] public string PhoneNumber { get; set; } = string.Empty;
        [Required, MaxLength(200)] public string EmergencyContactName { get; set; } = string.Empty;
        [Required, MaxLength(50)] public string EmergencyContactNumber { get; set; } = string.Empty;

        [Required, MaxLength(50)] public string Gender { get; set; } = string.Empty;
        [Required, MaxLength(100)] public string Nationality { get; set; } = string.Empty;
        [Required, MaxLength(50)] public string MaritalStatus { get; set; } = string.Empty;
        [Required, MaxLength(500)] public string Address { get; set; } = string.Empty;

        // Required job info
        [Required] public DateTime DateOfBirth { get; set; }
        [Required, MaxLength(150)] public string JobTitle { get; set; } = string.Empty;
        [Required, MaxLength(50)] public string EmployeeCode { get; set; } = string.Empty;
        [Required, MaxLength(100)] public string EmployeeEducationStatus { get; set; } = string.Empty;
        [Required, MaxLength(50)] public string EmploymentType { get; set; } = string.Empty;
        [Required, MaxLength(2083)] public string PhotoUrl { get; set; } = string.Empty;
        [Required] public DateTime JoiningDate { get; set; }

        // Required other fields
        [Required] public string BankDetails { get; set; } = string.Empty;
        [Required] public string CustomFields { get; set; } = "{}";
    }
}
