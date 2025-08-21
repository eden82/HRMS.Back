using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{
    public class EmployeeRole
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int EmployeeId { get; set; }
        public int DepartmentId { get; set; }

        // Navigation
        public Role Role { get; set; }
        public Employee Employee { get; set; }
        public Department Department { get; set; }
    }
}