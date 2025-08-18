using HRMS.Backend.Data;
using HRMS.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS.Backend.DTOs;


namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttendanceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAttendance([FromBody] Attendance attendance)
        {
            // Example shift start time
            var shiftStart = new TimeSpan(9, 0, 0);

            // 1. Decide Status
            if (attendance.ClockIn == default)
            {
                attendance.Status = "Absent";
            }
            else if (attendance.ClockIn.TimeOfDay > shiftStart)
            {
                attendance.Status = "Late";
            }
            else
            {
                attendance.Status = "Present";
            }

            // 2. Calculate total hours (if clock-out exists)
            if (attendance.ClockOut != null)
            {
                attendance.TotalHours = (attendance.ClockOut.Value - attendance.ClockIn).TotalHours;
            }

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return Ok(attendance);
        }



        



    }
}
