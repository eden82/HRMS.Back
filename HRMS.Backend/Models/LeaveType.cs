namespace HRMS.Backend.Models
{
    public class LeaveType
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }

        public string Name { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public bool CarryForward { get; set; }
        public string Description { get; set; } = string.Empty;
        public int MaxDays { get; set; }
        public bool RequiresApproval { get; set; }

        public Organization Organization { get; set; } = null!;
        public ICollection<Leave> Leaves { get; set; } = new List<Leave>();
    }
}
