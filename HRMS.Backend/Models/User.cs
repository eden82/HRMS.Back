using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{
    public class User
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("full_name"), Required, MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Column("email"), MaxLength(255)]
        public string? Email { get; set; }   // nullable to remove CS8601

        [Column("phone_number"), MaxLength(50)]
        public string? PhoneNumber { get; set; }  // nullable to remove CS8601

        [Column("password"), Required]
        public string Password { get; set; } = string.Empty;

        [Column("role"), MaxLength(50)]
        public string Role { get; set; } = "User";

        [Column("username"), Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        // <<< CHANGED to Guid? to match controllers >>>
        [Column("organization_id")]
        public Guid? OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
