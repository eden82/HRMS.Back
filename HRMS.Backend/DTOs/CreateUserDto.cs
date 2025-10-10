using System;

namespace HRMS.Backend.DTOs
{

    public sealed class CreateUserDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public Guid? TenantId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? EmployeeId { get; set; }
    }
}