using System;

namespace HRMS.Backend.DTOs
{
    // Read model
    public class TenantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string? Domain { get; set; }
        public string? Industry { get; set; }
        public string? Location { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Country { get; set; } = string.Empty;
        public string TimeZone { get; set; } = string.Empty;

        public bool EmployeeManagement { get; set; }
        public bool AttendanceTracking { get; set; }
        public bool LeaveManagement { get; set; }
        public bool Recruitment { get; set; }
        public bool PerformanceManagement { get; set; }
        public bool TrainingDevelopment { get; set; }

    }

    // Create
    public class CreateTenantDto
    {
        public string Name { get; set; } = string.Empty;

        public string? Domain { get; set; }
        public string? Industry { get; set; }
        public string? Location { get; set; }


        public string Country { get; set; } = string.Empty;
        public string TimeZone { get; set; } = string.Empty;

        public bool EmployeeManagement { get; set; }
        public bool AttendanceTracking { get; set; }
        public bool LeaveManagement { get; set; }
        public bool Recruitment { get; set; }
        public bool PerformanceManagement { get; set; }
        public bool TrainingDevelopment { get; set; }
    }

    // Update
    public class UpdateTenantDto : CreateTenantDto
    {
        public Guid Id { get; set; }
    }
}
