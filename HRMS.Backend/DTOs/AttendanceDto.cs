using System;

namespace HRMS.Backend.Models
{
    public sealed class AttendanceDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid TenantId { get; set; }
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
        public Guid EmployeeId { get; set; }
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
        public Guid EmployeeId { get; set; }
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
        public Guid? AttendanceId { get; set; }
        public Guid? EmployeeId { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public DateTime? ClockOut { get; set; }
    }
}
