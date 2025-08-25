namespace HRMS.Backend.Models
{
    public class OrgSetting
    {
        public long Id { get; set; }
        public int TenantId { get; set; }
        public int OrganizationId { get; set; }

        public string Settings { get; set; } = "{}";

        public int Version { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Organization Organization { get; set; } = null!;
    }
}
