public class OrgSetting
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }

    // JSONB stored as string
    public string Settings { get; set; }

    // Navigation
    public Organization Organization { get; set; }
}