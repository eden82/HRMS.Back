public class Goal
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int OrganizationId { get; set; }
    public int TenantId { get; set; }
    public string GoalTitle { get; set; }
    public string Category { get; set; }
    public string Priority { get; set; }
    public DateTime DueDate { get; set; }
    public string Description { get; set; }

    // Navigation
    public Employee Employee { get; set; }
    public Organization Organization { get; set; }
    public Tenant Tenant { get; set; }
}