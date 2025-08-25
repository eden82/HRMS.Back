using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{
    public class Tenant
    {
        [Key]
        public int Id { get; set; }

        [Column("name"), Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Column("domain"), MaxLength(255)]
        public string? Domain { get; set; }

        [Column("contact_email"), MaxLength(255)]
        public string? ContactEmail { get; set; }

        [Column("contact_phone"), MaxLength(50)]
        public string? ContactPhone { get; set; }

        [Column("address"), MaxLength(500)]
        public string? Address { get; set; }

        [Column("created_at"), DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at"), DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Organization> Organizations { get; set; } = new List<Organization>();
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<Department> Departments { get; set; } = new List<Department>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
