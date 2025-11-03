
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS.Backend.Data;
using HRMS.Backend.Models;
using HRMS.Backend.Filters;

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

        // GET: api/attendance
        [HttpGet]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetAll()
        {
            var list = await _context.Attendances
                .AsNoTracking()
                .Select(a => new AttendanceDto
                {
                    Id = a.Id,
                    EmployeeId = a.EmployeeId,
                    TenantId = a.TenantId,
                    AttendanceDate = a.AttendanceDate,
                    ClockIn = a.ClockIn,
                    ClockOut = a.ClockOut,
                    Status = a.Status,
                    Location = a.Location,
                    ShiftName = a.ShiftName,
                    Source = a.Source,
                    IpAddress = a.IpAddress,
                    ExceptionNote = a.ExceptionNote,
                    TotalHours = a.ClockIn.HasValue && a.ClockOut.HasValue
                        ? (a.ClockOut.Value - a.ClockIn.Value).TotalHours
                        : (double?)null
                })
                .ToListAsync();

            return Ok(list);
        }

        // GET: api/attendance/{id}
        [HttpGet("{id:guid}")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<ActionResult<AttendanceDto>> GetById(Guid id)
        {
            var a = await _context.Attendances.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (a == null) return NotFound();

            return Ok(new AttendanceDto
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                TenantId = a.TenantId,
                AttendanceDate = a.AttendanceDate,
                ClockIn = a.ClockIn,
                ClockOut = a.ClockOut,
                Status = a.Status,
                Location = a.Location,
                ShiftName = a.ShiftName,
                Source = a.Source,
                IpAddress = a.IpAddress,
                ExceptionNote = a.ExceptionNote,
                TotalHours = a.ClockIn.HasValue && a.ClockOut.HasValue
                    ? (a.ClockOut.Value - a.ClockIn.Value).TotalHours
                    : (double?)null
            });
        }

        // POST: api/attendance
        [HttpPost]
        [RoleAuthorize("Employee")]
        public async Task<ActionResult<AttendanceDto>> Create([FromBody] AttendanceCreateUpdateDto input)
        {
            if (input == null) return BadRequest("Body required.");
            if (input.EmployeeId == Guid.Empty) return BadRequest("EmployeeId is required.");

            // Load employee for org/tenant linkage
            var emp = await _context.Employees.AsNoTracking()
                         .FirstOrDefaultAsync(e => e.EmployeeID == input.EmployeeId);
            if (emp == null) return BadRequest("Employee not found.");

            // 🔒 Fix: ensure we pass Guid (not Guid?) into GetSettingsAsync
            if (!emp.OrganizationId.HasValue)
                return BadRequest("Employee has no OrganizationId.");
            var settings = await GetSettingsAsync(emp.TenantId, emp.OrganizationId.Value);
            if (settings == null) return BadRequest("Org settings not found for employee’s org.");

            var tz = GetTimeZone(settings.TimeZone);
            var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
            var attDate = input.AttendanceDate?.Date ?? nowLocal.Date;


            var entity = new Attendance
            {
                Id = Guid.NewGuid(),
                EmployeeId = emp.EmployeeID,
                TenantId = emp.TenantId,
                AttendanceDate = attDate,
                ClockIn = input.ClockIn ?? nowLocal,
                Status = null, // computed later (optional)
                Location = input.Location,
                ShiftName = input.ShiftName,
                Source = input.Source,
                IpAddress = input.IpAddress,
                ExceptionNote = input.ExceptionNote
            };

            // Optional: set status on create based on clock-in
            entity.Status = ComputeStatus(entity, settings, hasApprovedLeave: false);

            _context.Attendances.Add(entity);
            await _context.SaveChangesAsync();

            var dto = new AttendanceDto
            {
                Id = entity.Id,
                EmployeeId = entity.EmployeeId,
                TenantId = entity.TenantId,
                AttendanceDate = entity.AttendanceDate,
                ClockIn = entity.ClockIn,
                ClockOut = entity.ClockOut,
                Status = entity.Status,
                Location = entity.Location,
                ShiftName = entity.ShiftName,
                Source = entity.Source,
                IpAddress = entity.IpAddress,
                ExceptionNote = entity.ExceptionNote,
                TotalHours = null
            };

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, dto);
        }

        // PUT: api/attendance/{id}
        [HttpPut("{id:guid}")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AttendanceCreateUpdateDto input)
        {
            var a = await _context.Attendances.FirstOrDefaultAsync(x => x.Id == id);
            if (a == null) return NotFound();

            var emp = await _context.Employees.AsNoTracking()
                         .FirstOrDefaultAsync(e => e.EmployeeID == a.EmployeeId);
            if (emp == null) return BadRequest("Employee not found.");

            // 🔒 Fix: ensure Guid (not Guid?)
            if (!emp.OrganizationId.HasValue)
                return BadRequest("Employee has no OrganizationId.");
            var settings = await GetSettingsAsync(emp.TenantId, emp.OrganizationId.Value);
            if (settings == null) return BadRequest("Org settings not found for employee’s org.");

            if (input.AttendanceDate.HasValue) a.AttendanceDate = input.AttendanceDate.Value.Date;
            if (input.ClockIn.HasValue) a.ClockIn = input.ClockIn;
            if (input.ClockOut.HasValue) a.ClockOut = input.ClockOut;

            a.Status = ComputeStatus(a, settings, hasApprovedLeave: false);
            a.Location = input.Location ?? a.Location;
            a.ShiftName = input.ShiftName ?? a.ShiftName;
            a.Source = input.Source ?? a.Source;
            a.IpAddress = input.IpAddress ?? a.IpAddress;
            a.ExceptionNote = input.ExceptionNote ?? a.ExceptionNote;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/attendance/{id}
        [HttpDelete("{id:guid}")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var a = await _context.Attendances.FindAsync(id);
            if (a == null) return NotFound();

            _context.Attendances.Remove(a);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        //// POST: api/attendance/clockin
        //[HttpPost("clockin")]

        //public async Task<ActionResult<AttendanceDto>> ClockIn([FromBody] ClockInDto input)
        //{
        //    if (input == null) return BadRequest("Body required.");
        //    if (input.EmployeeId == Guid.Empty) return BadRequest("EmployeeId is required.");

        //    var emp = await _context.Employees.AsNoTracking()
        //                 .FirstOrDefaultAsync(e => e.EmployeeID == input.EmployeeId);
        //    if (emp == null) return BadRequest("Employee not found.");



        //    // 🔒 Fix: ensure Guid (not Guid?)
        //    if (!emp.OrganizationId.HasValue)
        //        return BadRequest("Employee has no OrganizationId.");
        //    var settings = await GetSettingsAsync(emp.TenantId, emp.OrganizationId.Value);
        //    if (settings == null) return BadRequest("Org settings not found for employee’s org.");

        //    var tz = GetTimeZone(settings.TimeZone);
        //    var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        //    var date = input.AttendanceDate?.Date ?? nowLocal.Date;

        //    var existing = await _context.Attendances
        //        .FirstOrDefaultAsync(a => a.EmployeeId == emp.EmployeeID &&
        //                                  a.TenantId == emp.TenantId &&
        //                                  a.AttendanceDate == date);
        //    if (existing != null && existing.ClockIn.HasValue)
        //        return Conflict(new { message = "Already clocked in for today." });

        //    if (existing == null)
        //    {
        //        existing = new Attendance
        //        {
        //            Id = Guid.NewGuid(),
        //            EmployeeId = emp.EmployeeID,
        //            TenantId = emp.TenantId,
        //            AttendanceDate = date
        //        };
        //        _context.Attendances.Add(existing);
        //    }

        //    existing.ClockIn = input.ClockIn ?? nowLocal;
        //    existing.Status = ComputeStatus(existing, settings, hasApprovedLeave: false);
        //    existing.Location = input.Location ?? existing.Location;
        //    existing.ShiftName = input.ShiftName ?? existing.ShiftName;
        //    existing.Source = input.Source ?? existing.Source;
        //    existing.IpAddress = input.IpAddress ?? existing.IpAddress;
        //    existing.ExceptionNote = input.ExceptionNote ?? existing.ExceptionNote;

        //    await _context.SaveChangesAsync();

        //    return Ok(new AttendanceDto
        //    {
        //        Id = existing.Id,
        //        EmployeeId = existing.EmployeeId,
        //        TenantId = existing.TenantId,
        //        AttendanceDate = existing.AttendanceDate,
        //        ClockIn = existing.ClockIn,
        //        ClockOut = existing.ClockOut,
        //        Status = existing.Status,
        //        Location = existing.Location,
        //        ShiftName = existing.ShiftName,
        //        Source = existing.Source,
        //        IpAddress = existing.IpAddress,
        //        ExceptionNote = existing.ExceptionNote,
        //        TotalHours = null
        //    });
        //}



        // POST: api/attendance/clockin
        [HttpPost("clockin")]
        public async Task<ActionResult<AttendanceDto>> ClockIn([FromBody] AttendanceDto input)
        {
            if (input == null) return BadRequest("Body required.");
            if (input.UserId == Guid.Empty) return BadRequest("UserId is required.");

            // 1️⃣ Find the user
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == input.UserId);

            if (user == null)
                return BadRequest("User not found.");

            // 2️⃣ Get EmployeeId from user
            if (user.EmployeeId == null)
                return BadRequest("User is not linked to an employee.");

            var employeeId = user.EmployeeId.Value;

            var emp = await _context.Employees
                .Include(e => e.Organization)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeID == employeeId);

            if (emp == null)
                return BadRequest("Employee not found.");

            var nowLocal = DateTime.Now;
            var date = input.AttendanceDate?.Date ?? nowLocal.Date;

            var existing = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == emp.EmployeeID &&
                                          a.TenantId == emp.TenantId &&
                                          a.AttendanceDate == date);

            if (existing != null && existing.ClockIn.HasValue)
                return Conflict(new { message = "Already clocked in for today." });

            if (existing == null)
            {
                existing = new Attendance
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = emp.EmployeeID,
                    TenantId = emp.TenantId,
                    OrganizationId = emp.OrganizationId,
                    AttendanceDate = date
                };
                _context.Attendances.Add(existing);
            }

            existing.ClockIn = input.ClockIn ?? nowLocal;
            existing.ShiftName = emp.ShiftDetails ?? input.ShiftName;
            existing.Location = emp.Organization?.Location ?? input.Location;
            existing.Source = input.Source ?? "System";
            existing.IpAddress = input.IpAddress ?? HttpContext.Connection.RemoteIpAddress?.ToString();
            existing.ExceptionNote = input.ExceptionNote ?? existing.ExceptionNote;

            // ✅ Create a temporary default OrgSetting (since you don't have one)
            var defaultSetting = new OrgSetting
            {
                WorkDayStart = new TimeSpan(9, 0, 0),    // 9:00 AM
                LateAfterMinutes = 15,                   // after 9:15 = Late
                HalfDayUnderHours = 4,                   // less than 4 hours = half day
                AbsentIfNoClockIn = true
            };

            // ✅ Compute the status dynamically
            existing.Status = ComputeStatus(existing, defaultSetting, hasApprovedLeave: false);

            await _context.SaveChangesAsync();

            return Ok(new AttendanceDto
            {
                Id = existing.Id,
                UserId = input.UserId,
                EmployeeId = existing.EmployeeId,
                TenantId = existing.TenantId,
                OrganizationId = existing.OrganizationId,
                AttendanceDate = existing.AttendanceDate,
                ClockIn = existing.ClockIn,
                ClockOut = existing.ClockOut,
                Status = existing.Status,
                Location = existing.Location,
                ShiftName = existing.ShiftName,
                Source = existing.Source,
                IpAddress = existing.IpAddress,
                ExceptionNote = existing.ExceptionNote,
                TotalHours = null
            });
        }


        // POST: api/attendance/clockout
        [HttpPost("clockout")]
        public async Task<ActionResult<AttendanceDto>> ClockOut([FromBody] AttendanceDto input)
        {
            if (input == null) return BadRequest("Body required.");
            if (input.UserId == Guid.Empty) return BadRequest("UserId is required.");

            // 1️⃣ Find the user
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == input.UserId);

            if (user == null)
                return BadRequest("User not found.");

            // 2️⃣ Get EmployeeId from user
            if (user.EmployeeId == null)
                return BadRequest("User is not linked to an employee.");

            var employeeId = user.EmployeeId.Value;

            var emp = await _context.Employees
                .Include(e => e.Organization)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeID == employeeId);

            if (emp == null)
                return BadRequest("Employee not found.");

            var nowLocal = DateTime.Now;
            var date = input.AttendanceDate?.Date ?? nowLocal.Date;

            var existing = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == emp.EmployeeID &&
                                          a.TenantId == emp.TenantId &&
                                          a.AttendanceDate == date);

            if (existing == null)
                return BadRequest("No clock-in record found for today.");

            if (existing.ClockOut.HasValue)
                return Conflict(new { message = "Already clocked out for today." });

            existing.ClockOut = input.ClockOut ?? nowLocal;
            existing.ExceptionNote = input.ExceptionNote ?? existing.ExceptionNote;

            // ✅ Same fallback default OrgSetting
            var defaultSetting = new OrgSetting
            {
                WorkDayStart = new TimeSpan(9, 0, 0),
                LateAfterMinutes = 15,
                HalfDayUnderHours = 4,
                AbsentIfNoClockIn = true
            };

            // ✅ Recompute the status (Late, Half Day, etc.)
            existing.Status = ComputeStatus(existing, defaultSetting, hasApprovedLeave: false);


            // Compute total hours
            if (existing.ClockIn.HasValue && existing.ClockOut.HasValue)
            {
                existing.TotalHours = (existing.ClockOut.Value - existing.ClockIn.Value).TotalHours;
            }


            await _context.SaveChangesAsync();

            return Ok(new AttendanceDto
            {
                Id = existing.Id,
                UserId = input.UserId,
                EmployeeId = existing.EmployeeId,
                TenantId = existing.TenantId,
                OrganizationId = existing.OrganizationId,
                AttendanceDate = existing.AttendanceDate,
                ClockIn = existing.ClockIn,
                ClockOut = existing.ClockOut,
                Status = existing.Status,
                Location = existing.Location,
                ShiftName = existing.ShiftName,
                Source = existing.Source,
                IpAddress = existing.IpAddress,
                ExceptionNote = existing.ExceptionNote,
                TotalHours = existing.TotalHours
            });
        }


                // GET: api/attendance/user/{userId}
                [HttpGet("user/{userId}")]
                public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetAttendanceByUser(Guid userId)
                {
                    if (userId == Guid.Empty)
                        return BadRequest("UserId is required.");

                    // 1️⃣ Find the user
                    var user = await _context.Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.Id == userId);

                    if (user == null)
                        return NotFound("User not found.");

                    // 2️⃣ Get EmployeeId from user
                    if (user.EmployeeId == null)
                        return BadRequest("User is not linked to an employee.");

                    var employeeId = user.EmployeeId.Value;

                    // 3️⃣ Get all attendance records for this employee
                    var attendances = await _context.Attendances
                        .Where(a => a.EmployeeId == employeeId)
                        .OrderByDescending(a => a.AttendanceDate)
                        .ToListAsync();

                    var attendanceDtos = attendances.Select(a => new AttendanceDto
                    {
                        Id = a.Id,
                        UserId = userId,                 // include userId
                        EmployeeId = a.EmployeeId,
                        TenantId = a.TenantId,
                        OrganizationId = a.OrganizationId,
                        AttendanceDate = a.AttendanceDate,
                        ClockIn = a.ClockIn,
                        ClockOut = a.ClockOut,
                        Status = a.Status,
                        Location = a.Location,
                        ShiftName = a.ShiftName,
                        Source = a.Source,
                        IpAddress = a.IpAddress,
                        ExceptionNote = a.ExceptionNote,
                        TotalHours = a.ClockIn.HasValue && a.ClockOut.HasValue
                            ? (a.ClockOut.Value - a.ClockIn.Value).TotalHours
                            : (double?)null
                    }).ToList();

                    return Ok(attendanceDtos);
                }


                // GET: api/attendance/today/stats
                [HttpGet("today/stats")]
                [RoleAuthorize("SuperAdmin,SystemAdmin,HR")]
                public async Task<ActionResult<object>> GetTodayAttendanceStats()
                {
                    var today = DateTime.Now.Date;

                    // Get all attendance records for today with employee info
                    var attendances = await _context.Attendances
                        .Include(a => a.Employee) // ensure Employee navigation property exists
                        .Where(a => a.AttendanceDate == today)
                        .AsNoTracking()
                        .ToListAsync();

                    // Count stats
                    var presentCount = attendances.Count(a => a.Status != null && a.Status.Contains("Present"));
                    var absentCount = attendances.Count(a => a.Status != null && a.Status.Contains("Absent"));
                    var lateCount = attendances.Count(a => a.Status != null && a.Status.Contains("Late"));

                    // Employees who clocked out
                    var clockedOutEmployees = attendances
                        .Where(a => a.ClockOut.HasValue)
                        .Select(a => new
                        {
                            FullName = $"{a.Employee?.FirstName} {a.Employee?.LastName}".Trim(),
                            a.ClockIn,
                            a.ClockOut,
                            a.TotalHours,
                            a.Status
                        })
                        .ToList();

                    return Ok(new
                    {
                        Date = today,
                        PresentToday = presentCount,
                        AbsentToday = absentCount,
                        LateToday = lateCount,
                        ClockedOutEmployees = clockedOutEmployees
                    });
                }

                // GET: api/attendance/today/stats/{tenantId}
                // Attendance stats by tenant only
                [HttpGet("today/stats/{tenantId}")]
                [RoleAuthorize("SuperAdmin,SystemAdmin,HR")]
                public async Task<ActionResult<object>> GetTodayAttendanceStatsByTenant(Guid tenantId)
                {
                    if (tenantId == Guid.Empty)
                        return BadRequest("Tenant ID must be provided.");

                    var today = DateTime.Now.Date;

                    var attendances = await _context.Attendances
                        .Include(a => a.Employee)
                        .Where(a => a.Employee.TenantId == tenantId && a.AttendanceDate == today)
                        .AsNoTracking()
                        .ToListAsync();

                    // Inline stats computation
                    var presentCount = attendances.Count(a => a.Status != null && a.Status.Contains("Present"));
                    var absentCount = attendances.Count(a => a.Status != null && a.Status.Contains("Absent"));
                    var lateCount = attendances.Count(a => a.Status != null && a.Status.Contains("Late"));

                    var clockedOutEmployees = attendances
                        .Where(a => a.ClockOut.HasValue)
                        .Select(a => new
                        {
                            FullName = $"{a.Employee?.FirstName} {a.Employee?.LastName}".Trim(),
                            a.ClockIn,
                            a.ClockOut,
                            a.TotalHours,
                            a.Status
                        })
                        .ToList();

                    return Ok(new
                    {
                        Date = today,
                        PresentToday = presentCount,
                        AbsentToday = absentCount,
                        LateToday = lateCount,
                        ClockedOutEmployees = clockedOutEmployees
                    });
                }


                // GET: api/attendance/today/stats/{tenantId}/{organizationId}
                // Attendance stats by tenant and organization
                [HttpGet("today/stats/{tenantId}/{organizationId}")]
                [RoleAuthorize("SuperAdmin,SystemAdmin,HR")]
                public async Task<ActionResult<object>> GetTodayAttendanceStatsByTenantAndOrg(Guid tenantId, Guid organizationId)
                {
                    if (tenantId == Guid.Empty)
                        return BadRequest("Tenant ID must be provided.");
                    if (organizationId == Guid.Empty)
                        return BadRequest("Organization ID must be provided.");

                    var today = DateTime.Now.Date;

                    var attendances = await _context.Attendances
                        .Include(a => a.Employee)
                        .Where(a => a.Employee.TenantId == tenantId
                                 && a.Employee.OrganizationId == organizationId
                                 && a.AttendanceDate == today)
                        .AsNoTracking()
                        .ToListAsync();

                    // Inline stats computation
                    var presentCount = attendances.Count(a => a.Status != null && a.Status.Contains("Present"));
                    var absentCount = attendances.Count(a => a.Status != null && a.Status.Contains("Absent"));
                    var lateCount = attendances.Count(a => a.Status != null && a.Status.Contains("Late"));

                    var clockedOutEmployees = attendances
                        .Where(a => a.ClockOut.HasValue)
                        .Select(a => new
                        {
                            FullName = $"{a.Employee?.FirstName} {a.Employee?.LastName}".Trim(),
                            a.ClockIn,
                            a.ClockOut,
                            a.TotalHours,
                            a.Status
                        })
                        .ToList();

                    return Ok(new
                    {
                        Date = today,
                        PresentToday = presentCount,
                        AbsentToday = absentCount,
                        LateToday = lateCount,
                        ClockedOutEmployees = clockedOutEmployees
                    });
                }



        //// PATCH: api/attendance/{id}/clockout
        //[HttpPatch("{id:guid}/clockout")]
        //public async Task<ActionResult<AttendanceDto>> ClockOut(Guid id, [FromBody] ClockOutDto input)
        //{
        //    var a = await _context.Attendances.FirstOrDefaultAsync(x => x.Id == id);
        //    if (a == null) return NotFound();

        //    var emp = await _context.Employees.AsNoTracking()
        //                 .FirstOrDefaultAsync(e => e.EmployeeID == a.EmployeeId);
        //    if (emp == null) return BadRequest("Employee not found.");

        //    // 🔒 Fix: ensure Guid (not Guid?)
        //    if (!emp.OrganizationId.HasValue)
        //        return BadRequest("Employee has no OrganizationId.");
        //    var settings = await GetSettingsAsync(emp.TenantId, emp.OrganizationId.Value);
        //    if (settings == null) return BadRequest("Org settings not found for employee’s org.");

        //    var tz = GetTimeZone(settings.TimeZone);
        //    var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

        //    a.ClockOut = input.ClockOut ?? nowLocal;
        //    a.Status = ComputeStatus(a, settings, hasApprovedLeave: false);

        //    await _context.SaveChangesAsync();



        //    return Ok(new AttendanceDto
        //    {
        //        Id = a.Id,
        //        EmployeeId = a.EmployeeId,
        //        TenantId = a.TenantId,
        //        AttendanceDate = a.AttendanceDate,
        //        ClockIn = a.ClockIn,
        //        ClockOut = a.ClockOut,
        //        Status = a.Status,
        //        Location = a.Location,
        //        ShiftName = a.ShiftName,
        //        Source = a.Source,
        //        IpAddress = a.IpAddress,
        //        ExceptionNote = a.ExceptionNote,
        //        TotalHours = a.ClockIn.HasValue && a.ClockOut.HasValue
        //            ? (a.ClockOut.Value - a.ClockIn.Value).TotalHours
        //            : (double?)null
        //    });
        //}

        // ===== Helpers =====

        private async Task<OrgSetting?> GetSettingsAsync(Guid tenantId, Guid organizationId)
        {
            return await _context.OrgSettings.AsNoTracking()
                .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.OrganizationId == organizationId);
        }

        private static TimeZoneInfo GetTimeZone(string tzId)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(string.IsNullOrWhiteSpace(tzId) ? "UTC" : tzId);
            }
            catch
            {
                return TimeZoneInfo.Utc;
            }
        }

        private static string ComputeStatus(Attendance a, OrgSetting s, bool hasApprovedLeave)
        {
            if (hasApprovedLeave) return "On Leave";

            // No clock-in yet: possibly Absent (policy) or Unknown
            if (!a.ClockIn.HasValue)
                return s.AbsentIfNoClockIn ? "Absent" : "Unknown";

            // Lateness
            var clockInLocal = a.ClockIn.Value;
            var workStart = clockInLocal.Date + s.WorkDayStart;
            var lateAfter = workStart.AddMinutes(s.LateAfterMinutes);

            var isLate = clockInLocal > lateAfter;

            // Half-day / full-day using hours worked (if clock-out known)
            if (a.ClockOut.HasValue)
            {
                var hours = (a.ClockOut.Value - a.ClockIn.Value).TotalHours;

                if (hours < Math.Max(0.1, s.HalfDayUnderHours)) // under threshold -> Half Day
                    return isLate ? "Late (Half Day)" : "Half Day";

                // Full day
                return isLate ? "Late (Present)" : "Present";
            }

            // No clock-out yet, but already late
            return isLate ? "Late" : "Present";
        }
    }

    // ====== DTOs used by this controller (align with your project’s namespace if needed) ======
    public sealed class AttendanceDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid TenantId { get; set; }
        public Guid? OrganizationId { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public DateTime? ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
        public string? Status { get; set; }
        public string? Location { get; set; }
        public string? ShiftName { get; set; }
        public string? Source { get; set; }
        public string? IpAddress { get; set; }
        public string? ExceptionNote { get; set; }
        public double? TotalHours { get; set; }
    }

    public sealed class AttendanceCreateUpdateDto
    {
        public Guid EmployeeId { get; set; }
        public Guid? OrganizationId { get; set; }
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
        public Guid? OrganizationId { get; set; }
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
        public DateTime? ClockOut { get; set; }
    }
}