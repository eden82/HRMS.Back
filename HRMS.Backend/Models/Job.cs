namespace HRMS.Backend.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public int TenantId { get; set; }

        public string? Location { get; set; }
        public string JobType { get; set; } = string.Empty;
        public string? SalaryRange { get; set; }
        public DateTime? ApplicationDeadline { get; set; }
        public string JobDescription { get; set; } = string.Empty;
        public string Requirement { get; set; } = string.Empty;
        public DateTime? ClosingDate { get; set; }

        public Department Department { get; set; } = null!;
        public Tenant Tenant { get; set; } = null!;
        public ICollection<Applicant> Applicants { get; set; } = new List<Applicant>();

    }
}
