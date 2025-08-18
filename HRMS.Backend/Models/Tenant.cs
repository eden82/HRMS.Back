using System.ComponentModel.DataAnnotations.Schema;


namespace HRMS.Backend.Models
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Domain { get; set; } = null!;
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Organization> Organizations { get; set; } = new List<Organization>();
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}