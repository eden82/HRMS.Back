namespace HRMS.Backend.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int? DepartmentId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedBy { get; set; }

        public Organization Organization { get; set; } = null!;
        public Department? Department { get; set; }
        public Employee Creator { get; set; } = null!;
    }
}
