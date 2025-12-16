using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Backend.DTOs
{
    public class ReviewQuestionCreateDto
    {
        [Required, MaxLength(500)]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        public Guid TenantId { get; set; }

        public Guid? OrganizationId { get; set; }   // null = tenant level
    }
}
