using Microsoft.AspNetCore.Mvc;
using HRMS.Backend.Data;
using HRMS.Backend.Models;
using HRMS.Backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveTypesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LeaveTypesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/leavetypes?orgId=...&isPaid=true
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid orgId, [FromQuery] bool? isPaid = null)
        {
            if (orgId == Guid.Empty)
                return BadRequest("OrganizationId is required.");

            var query = _context.LeaveTypes.AsQueryable().Where(l => l.OrganizationId == orgId);

            if (isPaid.HasValue)
                query = query.Where(l => l.IsPaid == isPaid.Value);

            var types = await query
                .AsNoTracking()
                .Select(l => new LeaveTypeViewDto
                {
                    Id = l.Id,
                    OrganizationId = l.OrganizationId,
                    Name = l.Name,
                    IsPaid = l.IsPaid,
                    CarryForward = l.CarryForward,
                    Description = l.Description,
                    MaxDays = l.MaxDays,
                    RequiresApproval = l.RequiresApproval
                })
                .ToListAsync();

            return Ok(types);
        }

        // GET: api/leavetypes/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var leaveType = await _context.LeaveTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leaveType == null)
                return NotFound();

            return Ok(new LeaveTypeViewDto
            {
                Id = leaveType.Id,
                OrganizationId = leaveType.OrganizationId,
                Name = leaveType.Name,
                IsPaid = leaveType.IsPaid,
                CarryForward = leaveType.CarryForward,
                Description = leaveType.Description,
                MaxDays = leaveType.MaxDays,
                RequiresApproval = leaveType.RequiresApproval
            });
        }

        //  POST: api/leavetypes (create new)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LeaveTypeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Leave type name is required.");

            // Prevent duplicates per organization
            var exists = await _context.LeaveTypes.AnyAsync(l =>
                l.OrganizationId == dto.OrganizationId &&
                l.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                return Conflict($"A leave type named '{dto.Name}' already exists for this organization.");

            var leaveType = new LeaveType
            {
                Id = Guid.NewGuid(),
                OrganizationId = dto.OrganizationId,
                Name = dto.Name.Trim(),
                IsPaid = dto.IsPaid,
                CarryForward = dto.CarryForward,
                Description = dto.Description,
                MaxDays = dto.MaxDays,
                RequiresApproval = dto.RequiresApproval
            };

            _context.LeaveTypes.Add(leaveType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = leaveType.Id }, leaveType);
        }

        // PUT: api/leavetypes/{id} (update)
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] LeaveTypeDto dto)
        {
            var leaveType = await _context.LeaveTypes.FindAsync(id);
            if (leaveType == null)
                return NotFound();

            leaveType.Name = dto.Name.Trim();
            leaveType.IsPaid = dto.IsPaid;
            leaveType.CarryForward = dto.CarryForward;
            leaveType.Description = dto.Description;
            leaveType.MaxDays = dto.MaxDays;
            leaveType.RequiresApproval = dto.RequiresApproval;

            await _context.SaveChangesAsync();
            return Ok(leaveType);
        }

        // DELETE: api/leavetypes/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var leaveType = await _context.LeaveTypes.FindAsync(id);
            if (leaveType == null)
                return NotFound();

            _context.LeaveTypes.Remove(leaveType);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
