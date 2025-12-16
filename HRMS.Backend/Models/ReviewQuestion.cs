using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Backend.Models
{
    public class ReviewQuestion
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        // Scope of the question
        [Required]
        public Guid TenantId { get; set; }

        public Guid? OrganizationId { get; set; } // null = tenant-wide

        // metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
