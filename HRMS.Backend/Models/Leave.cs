public class Leave
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int TenantId { get; set; }
    public int LeaveTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
    public int? ApprovedBy { get; set; }
    public string Reason { get; set; }
    public DateTime AppliedOn { get; set; }
    public string ManagerComment { get; set; }

    // Navigation
    public Employee Employee { get; set; }
    public Tenant Tenant { get; set; }
    public LeaveType LeaveType { get; set; }
    public Employee Approver { get; set; }
}
