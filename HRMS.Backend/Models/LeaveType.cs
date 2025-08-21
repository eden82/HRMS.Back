public class LeaveType
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public string Name { get; set; }
    public bool IsPaid { get; set; }
    public bool CarryForward { get; set; }
    public string Description { get; set; }
    public int MaxDays { get; set; }
    public bool RequiresApproval { get; set; }

    // Navigation
    public Organization Organization { get; set; }
    public ICollection<Leave> Leaves { get; set; }
}