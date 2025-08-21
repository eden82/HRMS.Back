public class TenantSetting
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    // JSONB stored as string
    public string Settings { get; set; }

    // Navigation
    public Tenant Tenant { get; set; }
}