using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{

    public class RequestFeedback
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime FeedbackDeadline { get; set; }
        public string FeedbackSources { get; set; }
        public string InstructionReviewers { get; set; }

        // Navigation
        public Employee Employee { get; set; }
    }
}