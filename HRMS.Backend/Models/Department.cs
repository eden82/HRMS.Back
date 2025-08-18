
namespace HRMS.Backend.Models
{
    public class Department
    {
        public int Id { get; set; }

        // Foreign key to Organization
        public int OrganizationId { get; set; }
        
        public Organization Organization { get; set; } = null!;

        public string DepartmentName { get; set; } = null!;

        public string DepartmentHead { get; set; } = null!;

        public int InitialEmployeeCount { get; set; }

        // Self-referencing foreign key for parent department (nullable)
        public int? ParentDepartmentId { get; set; }
       
        public Department ParentDepartment { get; set; } = null!;

        // Navigation property for child departments
        public ICollection<Department> ChildDepartments { get; set; } = new List<Department>();

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}