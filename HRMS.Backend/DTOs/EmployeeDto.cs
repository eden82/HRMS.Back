public class EmployeeDto
{
    public int EmployeeID { get; set; }

    // Personal Info
    public string? FirstName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Nationality { get; set; }
    public string? MaritalStatus { get; set; }

    // Contact Info
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? EmergencyContact { get; set; }

    // Job Info
    public string? JobTitle { get; set; }
    public string? EmploymentType { get; set; }
    public string? Manager { get; set; }
    public DateTime? JoiningDate { get; set; }
    public string? Salary { get; set; }
    public string? Currency { get; set; }
    public string? Role { get; set; }

    // Payment Info
    public string? PaymentMethod { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? TaxIdentificationNumber { get; set; }

    // Benefits & Documents (only paths)
    public string? BenefitsEnrollment { get; set; }
    public string? PassportNumber { get; set; }
    public string? ResumePath { get; set; }
    public string? ContractFilePath { get; set; }
    public string? CertificationPath { get; set; }

    // System Access (usually exclude password for security)
    public string? Username { get; set; }

    // Work Info
    public string? WorkLocation { get; set; }
    public string? ShiftDetails { get; set; }

    // Navigation info as *names* (to avoid cycles)
    
    public string? DepartmentName { get; set; }
    

    // Tracking
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
