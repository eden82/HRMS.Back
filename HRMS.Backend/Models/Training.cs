using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{

    public class Training
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int TenantId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Level { get; set; }
        public string Duration { get; set; }
        public string Instructor { get; set; }
        public int MaxEnrollment { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string VideoLink { get; set; }
        public string ContractFile { get; set; }
        public string Mode { get; set; }
        public int MaxParticipant { get; set; }

        // Navigation
        public Organization Organization { get; set; }
        public Tenant Tenant { get; set; }
        public ICollection<TrainingEnrollment> TrainingEnrollments { get; set; }
    }
}
