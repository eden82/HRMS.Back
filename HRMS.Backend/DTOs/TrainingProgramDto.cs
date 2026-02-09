using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Backend.DTOs
{
    // GET / API response DTO
    public class TrainingProgramDto

    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid? OrganizationId { get; set; }

        public string Title { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Level { get; set; } = null!;
        public int DurationHours { get; set; }

        // Instructor info
        public Guid InstructorId { get; set; }
        public string InstructorName { get; set; } = null!;

        public int MaxEnrollment { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
    }

    // POST / PUT DTO
    public class CreateTrainingProgramDto
    {
        [Required]
        public string Title { get; set; } = null!;

        public string Category { get; set; } = null!;

        // IMPORTANT: string, not enum
        public string Level { get; set; } = null!;

        public int DurationHours { get; set; }

        //  Instructor specified by email
        [Required]
        [EmailAddress]
        public string InstructorEmail { get; set; } = null!;

        public int MaxEnrollment { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string? Description { get; set; }

        [Required]
        public Guid TenantId { get; set; }
        // optional (null  tenant-level)
        public Guid? OrganizationId { get; set; }
    }
}
