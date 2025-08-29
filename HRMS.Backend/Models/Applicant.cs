using System.ComponentModel.DataAnnotations;

namespace HRMS.Backend.Models
{
    public class Applicant
    {
        public Guid Id { get; set; }

        // FK -> jobs(id)
        public Guid JobId { get; set; }
        public Job Job { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)] public string? Email { get; set; }
        [MaxLength(50)] public string? Phone { get; set; }
        [MaxLength(2083)] public string? ResumeUrl { get; set; }
        [MaxLength(50)] public string? Status { get; set; }
        [MaxLength(100)] public string? Source { get; set; }

        public string? Notes { get; set; } // nvarchar(max)
    }
}
