

namespace HRMS.Backend.DTOs
{
    public class AttendanceDto
    {
        public int AttendanceId { get; set; }
        public int EmployeeId { get; set; }
        public EmployeeDto Employee { get; set; }  // Now holds employee info
        public DateTime ClockIn { get; set; }
        public DateTime ClockOut { get; set; }
        public double TotalHours { get; set; }
        public string Status { get; set; }
    }
}