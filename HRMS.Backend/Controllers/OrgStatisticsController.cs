using HRMS.Backend.Models;
using HRMS.Backend.Data;
using HRMS.Backend.DTOs;
using HRMS.Backend.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrgStatisticsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrgStatisticsController(AppDbContext context)
        {
            _context = context;
        }

        // ===================== 1️⃣ ORGANIZATION STATISTICS =====================
        [HttpGet("organization/{tenantId}/{organizationId}")]
        public async Task<IActionResult> GetOrganizationStatistics(Guid tenantId, Guid organizationId)
        {
            var today = DateTime.UtcNow.Date;

            // --- Total employees in this org ---
            var totalEmployees = await _context.Employees
                .CountAsync(e => e.TenantId == tenantId && e.OrganizationId == organizationId);

            // --- Today's attendance count ---
            var todayPresent = await _context.Attendances
                .Where(a => a.TenantId == tenantId && a.AttendanceDate == today)
                .Join(_context.Employees,
                      a => a.EmployeeId,
                      e => e.EmployeeID,
                      (a, e) => new { a, e })
                .CountAsync(x => x.e.OrganizationId == organizationId && x.a.Status == "Present");

            // --- Each main department ---
            var mainDepartments = await _context.Departments
                .Where(d => d.TenantId == tenantId &&
                            d.OrganizationId == organizationId &&
                            d.ParentDepartmentId == null)
                .Select(d => new DepartmentAttendanceDto
                {
                    MainDepartmentName = d.DepartmentName,
                    EmployeeCount = d.Employees.Count(),
                    AttendancePercent = (
                        (double)d.Employees
                            .SelectMany(emp => emp.Attendances)
                            .Count(a => a.AttendanceDate == today && a.Status == "Present")
                        /
                        (d.Employees.Count() == 0 ? 1 : d.Employees.Count())
                    ) * 100
                })
                .ToListAsync();

            var result = new OrganizationStatisticsDto
            {
                OrganizationName = (await _context.Organizations
                    .Where(o => o.Id == organizationId)
                    .Select(o => o.Name)
                    .FirstOrDefaultAsync()) ?? "Unknown",
                TotalEmployees = totalEmployees,
                TodayPresent = todayPresent,
                MainDepartments = mainDepartments
            };

            return Ok(result);
        }

        // ===================== 2️⃣ TENANT STATISTICS =====================
        [HttpGet("stats/tenant/{tenantId}")]
        [RoleAuthorize("SuperAdmin,SystemAdmin,HR")]
        public async Task<IActionResult> GetTenantDepartmentStatistics(Guid tenantId)
        {
            var today = DateTime.UtcNow.Date;

            // ✅ Get employees under the tenant with NO organization
            var employees = await _context.Employees
                .Where(e => e.TenantId == tenantId && e.OrganizationId == null)
                .ToListAsync();

            var totalEmployees = employees.Count;

            // ✅ Get today's attendance for those employees
            var employeeIds = employees.Select(e => e.EmployeeID).ToList();

            var todayPresent = await _context.Attendances
                .Where(a => a.AttendanceDate == today && employeeIds.Contains(a.EmployeeId) && a.Status == "Present")
                .CountAsync();

            // ✅ Get main departments under the tenant (no organization)
            var mainDepartments = await _context.Departments
                .Where(d => d.TenantId == tenantId && d.OrganizationId == null && d.ParentDepartmentId == null)
                .ToListAsync();

            var departmentStats = new List<object>();

            // ✅ Loop through each main department and calculate stats
            foreach (var dept in mainDepartments)
            {
                var deptEmployees = employees.Where(e => e.DepartmentId == dept.Id).ToList();
                var deptEmployeeIds = deptEmployees.Select(e => e.EmployeeID).ToList();

                var deptAttendanceCount = await _context.Attendances
                    .Where(a => a.AttendanceDate == today && deptEmployeeIds.Contains(a.EmployeeId) && a.Status == "Present")
                    .CountAsync();

                decimal attendancePercent = deptEmployees.Count > 0
                    ? Math.Round((decimal)deptAttendanceCount / deptEmployees.Count * 100, 2)
                    : 0;

                departmentStats.Add(new
                {
                    mainDepartmentName = dept.DepartmentName,
                    employeeCount = deptEmployees.Count,
                    attendancePercent
                });
            }

            // ✅ Final result
            var result = new
            {
                totalEmployees,
                todayPresent,
                mainDepartments = departmentStats
            };

            return Ok(result); // ← make sure this line is inside the method!
        }




    }
}
