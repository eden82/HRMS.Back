using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{

    public class Applicant
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ResumeUrl { get; set; }
        public string Status { get; set; }
        public string Source { get; set; }
        public string Notes { get; set; }

        // Navigation
        public Job Job { get; set; }
        public ICollection<Interview> Interviews { get; set; }
    }
}