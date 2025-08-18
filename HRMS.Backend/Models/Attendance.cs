using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        [Required]
        public DateTime ClockIn { get; set; }

        public DateTime? ClockOut { get; set; }

        public double? TotalHours { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }   // backend will assign
    }

}
