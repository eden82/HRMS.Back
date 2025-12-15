namespace HRMS.Backend.DTOs
{
    public class InterviewDTO
    {
        public Guid Id { get; set; }
        public Guid ShortlistId { get; set; }
        public string ApplicantName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;

        public Guid? InterviewerId { get; set; }
        public string InterviewerName { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;   // Online/Offline
        public string Status { get; set; } = "Scheduled";

        public DateTime? ScheduledDate { get; set; }
        public TimeSpan? ScheduledTime { get; set; }
        public string? LocationORMeetingUrl { get; set; }
        public int? Duration { get; set; }
        public string InterviewNote { get; set; } = string.Empty;

        public string ApplicantEmail { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;

        public string InterviewerEmail { get; set; } = string.Empty;

    }

    public class InterviewCreateDto
    {
        public string ApplicantEmail { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;

        public string ScheduledDate { get; set; } = string.Empty; // "2025-12-03"
        public string ScheduledTime { get; set; } = string.Empty; // "14:30:00"

        public int Duration { get; set; } // minutes

        public string LocationOrMeetingUrl { get; set; } = string.Empty;

        public string? InterviewerEmail { get; set; } // Optional

        public string InterviewNote { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
    }

}
