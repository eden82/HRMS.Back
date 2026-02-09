using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{
    public class TrainingMaterial
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ProgramId { get; set; }
        public TrainingProgram Program { get; set; } = null!;

        [MaxLength(1000)]
        public string? DocumentOrVideoLink { get; set; } // e.g., YouTube or other link

        [MaxLength(255)]
        public string? ContractFileName { get; set; } // stored file name

        [MaxLength(500)]
        public string? ContractFilePath { get; set; } // file path in server storage

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
