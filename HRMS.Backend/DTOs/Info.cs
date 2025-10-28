using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Backend.DTOs
{

    public class DepartmentAttendanceDto
    {
        public string MainDepartmentName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public double AttendancePercent { get; set; }
    }

    public class OrganizationStatisticsDto
    {
        public string OrganizationName { get; set; } = string.Empty;
        public int TotalEmployees { get; set; }
        public int TodayPresent { get; set; }
        public List<DepartmentAttendanceDto> MainDepartments { get; set; } = new();
    }

    public class TenantStatisticsDto
    {
        public int TotalEmployees { get; set; }
        public int TodayPresent { get; set; }
        public List<OrganizationStatisticsDto> Organizations { get; set; } = new();
    }
}