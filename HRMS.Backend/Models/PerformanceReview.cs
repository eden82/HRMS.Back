using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{

    public class PerformanceReview
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int ReviewerId { get; set; }
        public short TechnicalSkill { get; set; }
        public int Communication { get; set; }
        public int Leadership { get; set; }
        public int Innovation { get; set; }
        public int Teamwork { get; set; }
        public short OverallFeedback { get; set; }
        public string ReviewCycle { get; set; }
        public DateTime ReviewPeriodStart { get; set; }
        public DateTime ReviewPeriodEnd { get; set; }

        // Navigation
        public Employee Employee { get; set; }
        public Employee Reviewer { get; set; }
    }
}