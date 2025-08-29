using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{
    public class Announcement
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }

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
