namespace HRMS.Backend.Models
{
    public class Role
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public Tenant Tenant { get; set; } = null!;
    }
}