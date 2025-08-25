using System;

namespace HRMS.Backend.Models
{
    public sealed class AttendanceDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int TenantId { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public DateTime? ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
        public string? Status { get; set; }
        public string? Location { get; set; }
        public string? ShiftName { get; set; }
        public string? Source { get; set; }
        public string? IpAddress { get; set; }
        public string? ExceptionNote { get; set; }
        public double? TotalHours { get; set; } // computed in controller
    }

    public sealed class AttendanceCreateUpdateDto
    {
        public int EmployeeId { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public DateTime? ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
        public string? Status { get; set; }
        public string? Location { get; set; }
        public string? ShiftName { get; set; }
        public string? Source { get; set; }
        public string? IpAddress { get; set; }
        public string? ExceptionNote { get; set; }
    }

    public sealed class ClockInDto
    {
        public int EmployeeId { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public DateTime? ClockIn { get; set; }
        public string? Status { get; set; }
        public string? Location { get; set; }
        public string? ShiftName { get; set; }
        public string? Source { get; set; }
        public string? IpAddress { get; set; }
        public string? ExceptionNote { get; set; }
    }

    public sealed class ClockOutDto
    {
        public int? AttendanceId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public DateTime? ClockOut { get; set; }
    }
}
