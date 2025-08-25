namespace HRMS.Backend.Models
{
    public class Goal
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int OrganizationId { get; set; }
        public int TenantId { get; set; }

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
