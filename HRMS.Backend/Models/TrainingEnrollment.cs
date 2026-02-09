using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{
    public class TrainingEnrollment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        public Guid? OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        [Required]
        public Guid ProgramId { get; set; }

        [ForeignKey(nameof(ProgramId))]
        public TrainingProgram TrainingProgram { get; set; } = null!;

        [Required]
        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        [MaxLength(500)]
        public string? EnrollmentNote { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        // New progress property (0-100)
        [Range(0, 100)]
        public int Progress { get; set; } = 0;

        // Navigation
        public TrainingFeedback? Feedback { get; set; }



    }
}
