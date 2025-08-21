public class Announcement
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public int DepartmentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }

    // Navigation
    public Organization Organization { get; set; }
    public Department Department { get; set; }
    public Employee Creator { get; set; }
}
