namespace HRMS.Backend.DTOs
{
    public class LeaveTypeDto
    {
        public Guid TenantId { get; set; }
        public Guid? OrganizationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public bool CarryForward { get; set; }
        public string? Description { get; set; }
        public int MaxDays { get; set; }
        public bool RequiresApproval { get; set; } = true;
    }

    public class LeaveTypeViewDto : LeaveTypeDto
    {
        public Guid Id { get; set; }
    }
}
