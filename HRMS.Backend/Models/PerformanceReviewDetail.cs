using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{
    public class PerformanceReviewDetail
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Main Review ID (Foreign Key)
        [Required]
        public Guid PerformanceReviewId { get; set; }
        public PerformanceReview PerformanceReview { get; set; } = null!;

        // Review Question
        [Required]
        public Guid ReviewQuestionId { get; set; }
        public ReviewQuestion ReviewQuestion { get; set; } = null!;

        // Rating/Score
        [Required]
        public int Rating { get; set; }

        [Required]
        public string FeedBack { get; set; } = string.Empty;

        [Required]
        public Guid TenantID { get; set; }
        public Tenant? Tenant { get; set; }

        public Guid? OrganizationID { get; set; }
        public Organization? Organization { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
