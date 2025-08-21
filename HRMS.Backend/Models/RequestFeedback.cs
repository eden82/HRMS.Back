public class RequestFeedback
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime FeedbackDeadline { get; set; }
    public string FeedbackSources { get; set; }
    public string InstructionReviewers { get; set; }

    // Navigation
    public Employee Employee { get; set; }
}