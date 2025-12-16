using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Backend.Models
{
    public class PerformanceReview
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();


        [Required]
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;


        [Required]
        public Guid TenantID { get; set; }
        public Tenant? Tenant { get; set; }

        public Guid? OrganizationID { get; set; }
        public Organization? Organization { get; set; }


        [Required, MaxLength(50)]
        public string ReviewType { get; set; } = string.Empty;

        [Required]
        public string OverallFeedback { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ReviewCycle { get; set; } = string.Empty;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public List<PerformanceReviewDetail> Details { get; set; } = new();
    }
}

