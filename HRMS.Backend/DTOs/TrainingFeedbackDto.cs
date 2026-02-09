using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Backend.DTOs
{
    public class TrainingFeedbackDto
    {
        public Guid Id { get; set; }

        public Guid ProgramId { get; set; }

        public Guid EnrollmentId { get; set; }

        public Guid EmployeeId { get; set; }

        public Guid TenantId { get; set; }

        public Guid? OrganizationId { get; set; }

        public string Feedback { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }

    public class CreateTrainingFeedbackDto
    {
        [Required]
        public Guid ProgramId { get; set; }

        [Required]
        public Guid EnrollmentId { get; set; }

        [Required]
        public string Feedback { get; set; } = null!;
    }
}
