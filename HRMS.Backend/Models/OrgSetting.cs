using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{

    public class OrgSetting
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }

        // JSONB stored as string
        public string Settings { get; set; }

        // Navigation
        public Organization Organization { get; set; }
    }
}