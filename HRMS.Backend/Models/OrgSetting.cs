namespace HRMS.Backend.Models
{
    public class OrgSetting
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid OrganizationId { get; set; }

        public string Settings { get; set; } = "{}";

        public int Version { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Organization Organization { get; set; } = null!;
    }
}
