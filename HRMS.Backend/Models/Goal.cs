namespace HRMS.Backend.Models
{
    public class Goal
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid TenantId { get; set; }

        public string GoalTitle { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public string Description { get; set; } = string.Empty;

        public Employee Employee { get; set; } = null!;
        public Organization Organization { get; set; } = null!;
        public Tenant Tenant { get; set; } = null!;
    }
}
