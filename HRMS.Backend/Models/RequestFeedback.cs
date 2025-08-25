namespace HRMS.Backend.Models
{
    public class RequestFeedback
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }

        public DateTime? FeedbackDeadline { get; set; }
        public string FeedbackSources { get; set; } = string.Empty;
        public string InstructionReviewers { get; set; } = string.Empty;

        public Employee Employee { get; set; } = null!;
    }
}
