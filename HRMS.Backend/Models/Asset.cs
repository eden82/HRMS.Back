namespace HRMS.Backend.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int TenantId { get; set; }

        public string AssetName { get; set; } = string.Empty;
        public int? AssignedTo { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? IssuedOn { get; set; }
        public DateTime? ReturnedOn { get; set; }
        public string ConditionNotes { get; set; } = string.Empty;
        public string? AssetTag { get; set; }
        public string Category { get; set; } = string.Empty;

        public Organization Organization { get; set; } = null!;
        public Tenant Tenant { get; set; } = null!;
        public Employee? Employee { get; set; }
    }
}
