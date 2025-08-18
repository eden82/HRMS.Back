namespace HRMS.Backend.DTOs
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public string DepartmentHead { get; set; } = null!;
        public int InitialEmployeeCount { get; set; }
        public int? ParentDepartmentId { get; set; }
    }
}
