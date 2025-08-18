using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; } 

        // Personal Info
        [Required, MaxLength(50)]
        public string? FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(50)]
        public string? Nationality { get; set; }

        [MaxLength(20)]
        public string? MaritalStatus { get; set; }

        // Contact Info
        [EmailAddress, MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? EmergencyContact { get; set; }

        // Job Info
        [MaxLength(100)]
        public string? JobTitle { get; set; }

        [MaxLength(50)]
        public string? EmploymentType { get; set; }

        [MaxLength(100)]
        public string? Manager { get; set; }

        public DateTime? JoiningDate { get; set; }

        [MaxLength(20)]
        public string? Salary { get; set; }

        [MaxLength(10)]
        public string? Role { get; set; }

        [MaxLength(100)]
        public string? Currency { get; set; }

        // Payment Info
        [MaxLength(50)]
        public string? PaymentMethod { get; set; }

        [MaxLength(50)]
        public string? BankAccountNumber { get; set; }

        [MaxLength(50)]
        public string? TaxIdentificationNumber { get; set; }

        // Benefits & Documents (Paths instead of Binary)
        [MaxLength(255)]
        public string? BenefitsEnrollment { get; set; }

        [MaxLength(50)]
        public string? PassportNumber { get; set; }

        [MaxLength(500)]
        public string? ResumePath { get; set; }

        [MaxLength(500)]
        public string? ContractFilePath { get; set; }

        [MaxLength(500)]
        public string? CertificationPath { get; set; }

        // System Access
        [MaxLength(50)]
        public string? Username { get; set; }

        [MaxLength(255)]
        public string? PasswordHash { get; set; }

        //[MaxLength(50)]
        //public string Role { get; set; }

        // Work Info
        [MaxLength(100)]
        public string? WorkLocation { get; set; }

        [MaxLength(255)]
        public string? ShiftDetails { get; set; }

        // Tracking
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
