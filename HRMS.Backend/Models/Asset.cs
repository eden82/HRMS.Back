public class Asset
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public int TenantId { get; set; }
    public string AssetName { get; set; }
    public int AssignedTo { get; set; }
    public string Status { get; set; }
    public DateTime IssuedOn { get; set; }
    public DateTime ReturnedOn { get; set; }
    public string ConditionNotes { get; set; }
    public string AssetTag { get; set; }
    public string Category { get; set; }

    // Navigation
    public Organization Organization { get; set; }
    public Tenant Tenant { get; set; }
    public Employee Employee { get; set; }
}