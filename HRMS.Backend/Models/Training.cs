namespace HRMS.Backend.Models
{
    public class Training
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int TenantId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public int? MaxEnrollment { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? VideoLink { get; set; }
        public string? ContractFile { get; set; }
        public string Mode { get; set; } = string.Empty;
        public int? MaxParticipant { get; set; }

        public Organization Organization { get; set; } = null!;
        public Tenant Tenant { get; set; } = null!;
        public ICollection<TrainingEnrollment> TrainingEnrollments { get; set; } = new List<TrainingEnrollment>();
    }
}
