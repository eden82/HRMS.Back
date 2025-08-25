using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{
    
    public class Department
    {
        public int Id { get; set; }

        [Column("organization_id")]
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;

        [Column("tenant_id")]
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        // Map to diagram's 'name'
        [Column("name"), Required, MaxLength(200)]
        public string DepartmentName { get; set; } = null!;

        [Column("description")]
        public string? Description { get; set; }

        [Column("department_code"), MaxLength(50)]
        public string? DepartmentCode { get; set; }

        [Column("parent_department_id")]
        public int? ParentDepartmentId { get; set; }
        public Department? ParentDepartment { get; set; }

        public ICollection<Department> ChildDepartments { get; set; } = new List<Department>();
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();

        // Your extras (kept)
        public string? DepartmentHead { get; set; }
        public int InitialEmployeeCount { get; set; }
    }
}
