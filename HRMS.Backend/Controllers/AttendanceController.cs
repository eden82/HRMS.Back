using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS.Backend.Data;
using HRMS.Backend.Models;

namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]   // /api/attendance (Controller name = AttendanceController)
    [Route("api/attendances")]    // also allow /api/attendances
    public class AttendanceController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AttendanceController(AppDbContext context) => _context = context;

        // GET /api/attendance?employeeId={guid}&date=2025-08-25
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> Query(
            [FromQuery] Guid? employeeId,
            [FromQuery] DateTime? date)
        {
            var q = _context.Attendances.AsNoTracking().AsQueryable();

            if (employeeId.HasValue)
                q = q.Where(a => a.EmployeeId == employeeId.Value);

            if (date.HasValue)
            {
                var d = date.Value.Date;
                q = q.Where(a => a.AttendanceDate == d);
            }

            var list = await q
                .OrderByDescending(a => a.AttendanceDate)
                .ThenByDescending(a => a.ClockIn)
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
                    TotalHours = (a.ClockIn.HasValue && a.ClockOut.HasValue)
                        ? (double?)(EF.Functions.DateDiffMinute(a.ClockIn.Value, a.ClockOut.Value) / 60.0)
                        : null
                })
                .ToListAsync();

            return Ok(list);
        }

        // GET /api/attendance/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AttendanceDto>> GetById(Guid id)
        {
            var dto = await _context.Attendances
                .AsNoTracking()
                .Where(x => x.Id == id)
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
                    TotalHours = (a.ClockIn.HasValue && a.ClockOut.HasValue)
                        ? (double?)(EF.Functions.DateDiffMinute(a.ClockIn.Value, a.ClockOut.Value) / 60.0)
                        : null
                })
                .FirstOrDefaultAsync();

            if (dto is null) return NotFound();
            return Ok(dto);
        }

        // POST /api/attendance
        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<AttendanceDto>> Create([FromBody] AttendanceCreateUpdateDto dto)
        {
            var emp = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeID == dto.EmployeeId);
            if (emp is null)
                return BadRequest(new { message = "Employee not found." });

            var attendance = new Attendance
            {
                Id = Guid.NewGuid(),
                EmployeeId = dto.EmployeeId,
                TenantId = emp.TenantId,
                AttendanceDate = (dto.AttendanceDate ?? DateTime.UtcNow).Date,
                ClockIn = dto.ClockIn,
                ClockOut = dto.ClockOut,
                Status = dto.Status,
                Location = dto.Location,
                ShiftName = dto.ShiftName,
                Source = dto.Source,
                IpAddress = dto.IpAddress,
                ExceptionNote = dto.ExceptionNote
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = attendance.Id }, ToDto(attendance));
        }

        // PUT /api/attendance/{id}
        [HttpPut("{id:guid}")]
        [Consumes("application/json")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AttendanceCreateUpdateDto dto)
        {
            var a = await _context.Attendances.FirstOrDefaultAsync(x => x.Id == id);
            if (a is null) return NotFound();

            var emp = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeID == dto.EmployeeId);
            if (emp is null)
                return BadRequest(new { message = "Employee not found." });

            a.EmployeeId = dto.EmployeeId;
            a.TenantId = emp.TenantId;
            a.AttendanceDate = (dto.AttendanceDate ?? a.AttendanceDate)?.Date;
            a.ClockIn = dto.ClockIn;
            a.ClockOut = dto.ClockOut;
            a.Status = dto.Status;
            a.Location = dto.Location;
            a.ShiftName = dto.ShiftName;
            a.Source = dto.Source;
            a.IpAddress = dto.IpAddress;
            a.ExceptionNote = dto.ExceptionNote;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /api/attendance/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var a = await _context.Attendances.FirstOrDefaultAsync(x => x.Id == id);
            if (a is null) return NotFound();

            _context.Attendances.Remove(a);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST /api/attendance/clock-in
        [HttpPost("clock-in")]
        [Consumes("application/json")]
        public async Task<ActionResult<AttendanceDto>> ClockIn([FromBody] ClockInDto dto)
        {
            var emp = await _context.Employees.AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeID == dto.EmployeeId);
            if (emp is null) return BadRequest(new { message = "Employee not found." });

            var today = (dto.AttendanceDate ?? DateTime.UtcNow).Date;

            var a = await _context.Attendances
                .FirstOrDefaultAsync(x => x.EmployeeId == dto.EmployeeId &&
                                          x.TenantId == emp.TenantId &&
                                          x.AttendanceDate == today);

            if (a is null)
            {
                a = new Attendance
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = dto.EmployeeId,
                    TenantId = emp.TenantId,
                    AttendanceDate = today,
                    ClockIn = dto.ClockIn ?? DateTime.UtcNow,
                    Status = dto.Status ?? "Present",
                    Source = dto.Source,
                    Location = dto.Location,
                    IpAddress = dto.IpAddress,
                    ShiftName = dto.ShiftName,
                    ExceptionNote = dto.ExceptionNote
                };
                _context.Attendances.Add(a);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = a.Id }, ToDto(a));
            }

            if (a.ClockIn.HasValue)
                return Conflict(new { message = "Already clocked in for this date." });

            a.ClockIn = dto.ClockIn ?? DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(dto.Status)) a.Status = dto.Status;
            a.Source = dto.Source ?? a.Source;
            a.Location = dto.Location ?? a.Location;
            a.IpAddress = dto.IpAddress ?? a.IpAddress;
            a.ShiftName = dto.ShiftName ?? a.ShiftName;
            a.ExceptionNote = dto.ExceptionNote ?? a.ExceptionNote;

            await _context.SaveChangesAsync();
            return Ok(ToDto(a));
        }

        // POST /api/attendance/clock-out
        [HttpPost("clock-out")]
        [Consumes("application/json")]
        public async Task<ActionResult<AttendanceDto>> ClockOut([FromBody] ClockOutDto dto)
        {
            var when = dto.ClockOut ?? DateTime.UtcNow;

            Attendance? a = null;

            if (dto.AttendanceId.HasValue)
            {
                a = await _context.Attendances.FirstOrDefaultAsync(x => x.Id == dto.AttendanceId.Value);
            }
            else
            {
                if (!dto.EmployeeId.HasValue)
                    return BadRequest(new { message = "EmployeeId or AttendanceId is required." });

                var emp = await _context.Employees.AsNoTracking()
                    .FirstOrDefaultAsync(e => e.EmployeeID == dto.EmployeeId.Value);
                if (emp is null) return BadRequest(new { message = "Employee not found." });

                var date = (dto.AttendanceDate ?? when).Date;
                a = await _context.Attendances
                    .FirstOrDefaultAsync(x => x.EmployeeId == dto.EmployeeId.Value &&
                                              x.TenantId == emp.TenantId &&
                                              x.AttendanceDate == date);
            }

            if (a is null) return NotFound(new { message = "Attendance record not found." });
            if (a.ClockOut.HasValue) return Conflict(new { message = "Already clocked out." });

            a.ClockOut = when;
            if (string.IsNullOrWhiteSpace(a.Status)) a.Status = "Present";

            await _context.SaveChangesAsync();
            return Ok(ToDto(a));
        }

        private static AttendanceDto ToDto(Attendance a) => new AttendanceDto
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
            TotalHours = (a.ClockIn.HasValue && a.ClockOut.HasValue)
                ? (double?)((a.ClockOut.Value - a.ClockIn.Value).TotalHours)
                : null
        };
    }

    // ===== DTOs (GUID) =====
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
        public double? TotalHours { get; set; }
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
