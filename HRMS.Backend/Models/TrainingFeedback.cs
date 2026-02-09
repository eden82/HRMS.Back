using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{
    public class TrainingFeedback
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

        [Required]
        public Guid EnrollmentId { get; set; }

        [ForeignKey(nameof(EnrollmentId))]
        public TrainingEnrollment Enrollment { get; set; } = null!;

        [Required]
        [MaxLength(1000)]
        public string Feedback { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
