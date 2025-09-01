using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Backend.Data;
using HRMS.Backend.DTOs;
using HRMS.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/attendance")]
    public class AttendanceController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AttendanceController(AppDbContext context) => _context = context;

        // GET: /api/attendance
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetAll()
        {
            var rows = await _context.Attendances.AsNoTracking()
                .OrderByDescending(a => a.AttendanceDate)
                .Take(500) // safety cap; adjust as you like
                .ToListAsync();

            return Ok(rows.Select(Map));
        }

        // GET: /api/attendance/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AttendanceDto>> GetById(Guid id)
        {
            var a = await _context.Attendances.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (a == null) return NotFound();
            return Ok(Map(a));
        }

        // POST: /api/attendance  (create arbitrary row, optionally with ClockIn)
        [HttpPost]
        public async Task<ActionResult<AttendanceDto>> Create([FromBody] AttendanceCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // validate employee exists in tenant
            var empExists = await _context.Employees
                .AnyAsync(e => e.EmployeeID == dto.EmployeeId && e.TenantId == dto.TenantId);
            if (!empExists)
                return BadRequest("Employee not found in the specified tenant.");

            var date = (dto.AttendanceDate ?? DateTime.UtcNow.Date).Date;

            // Enforce 1 record per employee per date (if you need multiple, remove this)
            var exists = await _context.Attendances
                .AnyAsync(a => a.EmployeeId == dto.EmployeeId && a.TenantId == dto.TenantId && a.AttendanceDate == date);
            if (exists)
                return Conflict(new { message = "Attendance for this employee and date already exists." });

            var row = new Attendance
            {
                Id = Guid.NewGuid(),
                EmployeeId = dto.EmployeeId,
                TenantId = dto.TenantId,
                AttendanceDate = date,
                ClockIn = dto.ClockIn ?? null,
                Status = dto.Status,
                Location = dto.Location,
                ShiftName = dto.ShiftName,
                Source = dto.Source,
                IpAddress = dto.IpAddress,
                ExceptionNote = dto.ExceptionNote
            };

            // validate time logic
            if (row.ClockIn.HasValue && row.ClockIn.Value.Date != date)
                return BadRequest("ClockIn date must match AttendanceDate.");

            _context.Attendances.Add(row);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = row.Id }, Map(row));
        }

        // POST: /api/attendance/clockin
        [HttpPost("clockin")]
        public async Task<ActionResult<AttendanceDto>> ClockIn([FromBody] ClockInDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var emp = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeID == dto.EmployeeId && e.TenantId == dto.TenantId);

            if (emp == null) return BadRequest("Employee not found in the specified tenant.");

            var date = (dto.AttendanceDate ?? DateTime.UtcNow.Date).Date;
            var now = dto.ClockIn ?? DateTime.UtcNow;

            var existing = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == dto.EmployeeId && a.TenantId == dto.TenantId && a.AttendanceDate == date);

            if (existing != null)
            {
                // If already clocked in and not clocked out — block
                if (existing.ClockIn.HasValue && !existing.ClockOut.HasValue)
                    return Conflict(new { message = "Open attendance already exists (clocked in but not clocked out)." });

                // Otherwise update same row with a new clock-in
                existing.ClockIn = now;
                existing.Status = existing.Status ?? "Present";
                existing.Location = dto.Location ?? existing.Location;
                existing.ShiftName = dto.ShiftName ?? existing.ShiftName;
                existing.Source = dto.Source ?? existing.Source;
                existing.IpAddress = dto.IpAddress ?? existing.IpAddress;
                existing.ExceptionNote = dto.ExceptionNote ?? existing.ExceptionNote;

                await _context.SaveChangesAsync();
                return Ok(Map(existing));
            }

            var row = new Attendance
            {
                Id = Guid.NewGuid(),
                EmployeeId = dto.EmployeeId,
                TenantId = dto.TenantId,
                AttendanceDate = date,
                ClockIn = now,
                Status = "Present",
                Location = dto.Location,
                ShiftName = dto.ShiftName,
                Source = dto.Source,
                IpAddress = dto.IpAddress,
                ExceptionNote = dto.ExceptionNote
            };

            _context.Attendances.Add(row);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = row.Id }, Map(row));
        }

        // PATCH: /api/attendance/{id}/clockout
        [HttpPatch("{id:guid}/clockout")]
        public async Task<ActionResult<AttendanceDto>> ClockOut(Guid id, [FromBody] ClockOutDto dto)
        {
            var row = await _context.Attendances.FirstOrDefaultAsync(a => a.Id == id);
            if (row == null) return NotFound();

            if (!row.ClockIn.HasValue)
                return BadRequest("Cannot clock out a record without a clock-in.");

            var when = dto.ClockOut ?? DateTime.UtcNow;
            if (when < row.ClockIn.Value)
                return BadRequest("ClockOut cannot be earlier than ClockIn.");

            row.ClockOut = when;
            if (!string.IsNullOrWhiteSpace(dto.ExceptionNote))
                row.ExceptionNote = dto.ExceptionNote;

            await _context.SaveChangesAsync();
            return Ok(Map(row));
        }

        // PUT: /api/attendance/{id}  (admin fix)
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AttendanceUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch.");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var row = await _context.Attendances.FirstOrDefaultAsync(a => a.Id == id);
            if (row == null) return NotFound();

            // Ensure employee exists in tenant
            var empExists = await _context.Employees
                .AnyAsync(e => e.EmployeeID == dto.EmployeeId && e.TenantId == dto.TenantId);
            if (!empExists)
                return BadRequest("Employee not found in the specified tenant.");

            var date = dto.AttendanceDate.Date;

            // If changing date/employee, enforce uniqueness per day
            var duplicate = await _context.Attendances
                .AnyAsync(a => a.Id != id &&
                               a.EmployeeId == dto.EmployeeId &&
                               a.TenantId == dto.TenantId &&
                               a.AttendanceDate == date);
            if (duplicate)
                return Conflict(new { message = "Attendance for this employee and date already exists." });

            // Validate times
            if (dto.ClockIn.HasValue && dto.ClockIn.Value.Date != date)
                return BadRequest("ClockIn date must match AttendanceDate.");
            if (dto.ClockOut.HasValue && dto.ClockIn.HasValue && dto.ClockOut.Value < dto.ClockIn.Value)
                return BadRequest("ClockOut cannot be earlier than ClockIn.");

            row.EmployeeId = dto.EmployeeId;
            row.TenantId = dto.TenantId;
            row.AttendanceDate = date;
            row.ClockIn = dto.ClockIn;
            row.ClockOut = dto.ClockOut;
            row.Status = dto.Status;
            row.Location = dto.Location;
            row.ShiftName = dto.ShiftName;
            row.Source = dto.Source;
            row.IpAddress = dto.IpAddress;
            row.ExceptionNote = dto.ExceptionNote;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: /api/attendance/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var row = await _context.Attendances.FirstOrDefaultAsync(a => a.Id == id);
            if (row == null) return NotFound();

            _context.Attendances.Remove(row);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static AttendanceDto Map(Attendance a) => new AttendanceDto
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
            ExceptionNote = a.ExceptionNote
        };
    }
}
