using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Backend.Models
{
    public class TrainingProgram
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();


        //Foreign Keys
        [Required]
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        public Guid? OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        [Required]
        public Guid InstructorId { get; set; }
        public Employee Instructor { get; set; } = null!;


        // Properties
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Level { get; set; } = null!;


        public int DurationHours { get; set; }


        public int MaxEnrollment { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<TrainingEnrollment> Enrollments { get; set; } = new List<TrainingEnrollment>();
        public ICollection<TrainingFeedback> Feedbacks { get; set; } = new List<TrainingFeedback>();
        public ICollection<TrainingMaterial> TrainingMaterials { get; set; } = new List<TrainingMaterial>();
    }
}
