using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{

    public class TrainingEnrollment
    {
        public int Id { get; set; }
        public int TrainingId { get; set; }
        public int EmployeeId { get; set; }
        public string AttendanceStatus { get; set; }
        public string Feedback { get; set; }
        public DateTime EnrolledOn { get; set; }

        // Navigation
        public Training Training { get; set; }
        public Employee Employee { get; set; }
    }
}