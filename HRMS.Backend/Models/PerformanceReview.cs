namespace HRMS.Backend.Models
{
    public class PerformanceReview
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid ReviewerId { get; set; }

        public short TechnicalSkill { get; set; }
        public int Communication { get; set; }
        public int Leadership { get; set; }
        public int Innovation { get; set; }
        public int Teamwork { get; set; }
        public short OverallFeedback { get; set; }

        public string ReviewCycle { get; set; } = string.Empty;
        public string? ReviewPeriodStart { get; set; } // if these are datetime in DB, use DateTime?
        public string? ReviewPeriodEnd { get; set; }

        public Employee Employee { get; set; } = null!;
        public Employee Reviewer { get; set; } = null!;
    }
}
