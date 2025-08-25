namespace HRMS.Backend.Models
{
    public class EmployeeRole
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int EmployeeId { get; set; }
        public int DepartmentId { get; set; }

        public Role Role { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public Department Department { get; set; } = null!;
    }
}
