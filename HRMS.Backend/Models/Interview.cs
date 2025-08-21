public class Interview
{
    public int Id { get; set; }
    public int ApplicantId { get; set; }
    public DateTime ScheduledOn { get; set; }
    public int Duration { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string LocationUrl { get; set; }
    public string MeetingUrl { get; set; }
    public int InterviewerId { get; set; }
    public string InterviewNote { get; set; }
    public string Status { get; set; }
    public string Mode { get; set; }

    // Navigation
    public Applicant Applicant { get; set; }
    public Employee Interviewer { get; set; }
}