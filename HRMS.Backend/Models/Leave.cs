namespace HRMS.Backend.Models
{
    public class Leave
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int TenantId { get; set; }
        public int LeaveTypeId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Status { get; set; } = string.Empty;
        public int? ApprovedBy { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime? AppliedOn { get; set; }
        public string ManagerComment { get; set; } = string.Empty;

        public Employee Employee { get; set; } = null!;
        public Tenant Tenant { get; set; } = null!;
        public LeaveType LeaveType { get; set; } = null!;
        public Employee? Approver { get; set; }
    }
}
