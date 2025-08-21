public class Job
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int DepartmentId { get; set; }
    public int TenantId { get; set; }
    public string Location { get; set; }
    public string JobType { get; set; }
    public string SalaryRange { get; set; }
    public DateTime ApplicationDeadline { get; set; }
    public string JobDescription { get; set; }
    public string Requirement { get; set; }
    public DateTime ClosingDate { get; set; }

    // Navigation
    public Department Department { get; set; }
    public Tenant Tenant { get; set; }
    public ICollection<Applicant> Applicants { get; set; }
}