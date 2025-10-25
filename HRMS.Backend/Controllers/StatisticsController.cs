using HRMS.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS.Backend.Data;

namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StatisticsController(AppDbContext context)
        {
            _context = context;
        }



        //module-usage
        [HttpGet("module-usage")]
        public async Task<IActionResult> GetModuleUsage()
        {
            int totalTenants = await _context.Tenants.CountAsync();
            if (totalTenants == 0) return Ok(new { Message = "No tenants found" });

            var stats = new
            {
                EmployeeManagement = (double)await _context.Tenants.CountAsync(t => t.EmployeeManagement) / totalTenants * 100,
                AttendanceTracking = (double)await _context.Tenants.CountAsync(t => t.AttendanceTracking) / totalTenants * 100,
                LeaveManagement = (double)await _context.Tenants.CountAsync(t => t.LeaveManagement) / totalTenants * 100,
                Recruitment = (double)await _context.Tenants.CountAsync(t => t.Recruitment) / totalTenants * 100,
                PerformanceManagement = (double)await _context.Tenants.CountAsync(t => t.PerformanceManagement) / totalTenants * 100,
                TrainingDevelopment = (double)await _context.Tenants.CountAsync(t => t.TrainingDevelopment) / totalTenants * 100
            };

            return Ok(stats);
        }



        //top-organizations
        [HttpGet("top-organizations")]
        public async Task<IActionResult> GetTopOrganizations()
        {
            var tenants = await _context.Tenants
                .Select(t => new
                {
                    t.Name,
                    t.Domain,
                    ActiveUsers = t.Employees.Count,
                    ModulesEnabled = new bool[]
                    {
                t.EmployeeManagement,
                t.AttendanceTracking,
                t.LeaveManagement,
                t.Recruitment,
                t.PerformanceManagement,
                t.TrainingDevelopment
                    }.Count(x => x)
                })
                .OrderByDescending(x => x.ModulesEnabled)
                .Take(5)
                .ToListAsync();

            var results = tenants.Select(t => new
            {
                t.Name,
                t.Domain,
                t.ActiveUsers,
                UsageLevel = t.ModulesEnabled >= 5 ? "High Usage"
                            : t.ModulesEnabled >= 3 ? "Medium Usage"
                            : "Low Usage"
            });

            return Ok(results);
        }


        [HttpGet("active-users/total")]
        public async Task<IActionResult> GetTotalActiveUsers()
        {
            int totalActiveUsers = await _context.Users.CountAsync(u => u.IsActive);

            return Ok(new
            {
                TotalActiveUsers = totalActiveUsers
            });
        }


    }
}
