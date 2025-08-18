namespace HRMS.Backend.DTOs
{
    public class CreateOrganizationDto
    {
        public int TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Domain { get; set; }
        public string? Industry { get; set; }
        public string? CompanySize { get; set; }
        public string? Description { get; set; }
        public string? CountryCode { get; set; }
        public string? TimeZone { get; set; }

        // Admin
        public string AdminFirstName { get; set; } = string.Empty;
        public string AdminLastName { get; set; } = string.Empty;
        public string AdminEmail { get; set; } = string.Empty;
        public string? AdminPhone { get; set; }

        // Modules
        public bool EmployeeManagement { get; set; }
        public bool AttendanceTimeTracking { get; set; }
        public bool LeaveManagement { get; set; }
        public bool RecruitmentATS { get; set; }
        public bool PerformanceManagement { get; set; }
        public bool TrainingDevelopment { get; set; }
    }
}
