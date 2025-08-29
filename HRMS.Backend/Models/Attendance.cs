using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Backend.Models
{
    [Table("attendance")]
    public class Attendance
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        [Column("employee_id")]
        public Guid EmployeeId { get; set; }

        [Column("tenant_id")]
        public Guid TenantId { get; set; }

        [Column("attendance_date", TypeName = "date")]
        public DateTime? AttendanceDate { get; set; }

        [Column("clock_in")]
        public DateTime? ClockIn { get; set; }

        [Column("clock_out")]
        public DateTime? ClockOut { get; set; }

        [Column("status")]
        [MaxLength(50)]
        public string? Status { get; set; }

        [Column("location")]
        [MaxLength(200)]
        public string? Location { get; set; }

        [Column("shift_name")]
        [MaxLength(100)]
        public string? ShiftName { get; set; }

        [Column("source")]
        [MaxLength(50)]
        public string? Source { get; set; }

        [Column("ip_address")]
        [MaxLength(50)]
        public string? IpAddress { get; set; }

        [Column("exception_note")]
        public string? ExceptionNote { get; set; }

        // Navigation properties (ensure Employee and Tenant are in the same namespace)
        public Employee? Employee { get; set; }
        public Tenant? Tenant { get; set; }
    }
}
