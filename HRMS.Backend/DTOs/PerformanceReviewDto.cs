using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Backend.DTOs
{
    public class PerformanceReviewCreateDto
    {
        [Required]
        public string EmployeeEmail { get; set; } = string.Empty;

        [Required]
        public Guid TenantId { get; set; }

        public Guid? OrganizationId { get; set; } // Null for tenant-level review

        [Required, MaxLength(50)]
        public string ReviewType { get; set; } = string.Empty;

        [Required]
        public string OverallFeedback { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ReviewCycle { get; set; } = string.Empty;

        public List<PerformanceReviewQuestionDto> Questions { get; set; } = new();

    }

    public class PerformanceReviewQuestionDto
    {
        public Guid ReviewQuestionId { get; set; }
        public int Rating { get; set; }

        [Required]
        public string FeedBack { get; set; } = string.Empty;
    }

}
