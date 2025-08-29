namespace HRMS.Backend.Models
{
    public class TrainingEnrollment
    {
        public Guid Id { get; set; }
        public Guid TrainingId { get; set; }
        public Guid EmployeeId { get; set; }

        public string AttendanceStatus { get; set; } = string.Empty;
        public string Feedback { get; set; } = string.Empty;
        public DateTime? EnrolledOn { get; set; }

        public Training Training { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
    }
}
