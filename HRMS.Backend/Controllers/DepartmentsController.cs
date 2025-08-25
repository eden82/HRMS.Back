using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS.Backend.Models;
using HRMS.Backend.Data;
using HRMS.Backend.DTOs; // include namespace for DTO

namespace HRMS.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DepartmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
        {
            var departments = await _context.Departments
                .AsNoTracking()
                .ToListAsync();

            var departmentDtos = departments.Select(d => new DepartmentDto
            {
                Id = d.Id,
                OrganizationId = d.OrganizationId,
                DepartmentName = d.DepartmentName,
                DepartmentHead = d.DepartmentHead ?? string.Empty, // avoid CS8601
                InitialEmployeeCount = d.InitialEmployeeCount,
                ParentDepartmentId = d.ParentDepartmentId
            }).ToList();

            return Ok(departmentDtos);
        }

        // GET: api/departments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDto>> GetDepartmentById(int id)
        {
            var department = await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null) return NotFound();

            var dto = new DepartmentDto
            {
                Id = department.Id,
                OrganizationId = department.OrganizationId,
                DepartmentName = department.DepartmentName,
                DepartmentHead = department.DepartmentHead ?? string.Empty, // avoid CS8601
                InitialEmployeeCount = department.InitialEmployeeCount,
                ParentDepartmentId = department.ParentDepartmentId
            };

            return Ok(dto);
        }

        // POST: api/departments
        [HttpPost]
        public async Task<ActionResult<DepartmentDto>> CreateDepartment(DepartmentDto dto)
        {
            // Basic required checks
            if (string.IsNullOrWhiteSpace(dto.DepartmentName))
                ModelState.AddModelError(nameof(dto.DepartmentName), "Department name is required.");

            // Org must exist; derive tenant from it (schema rule)
            var org = await _context.Organizations
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == dto.OrganizationId);

            if (org == null)
                ModelState.AddModelError(nameof(dto.OrganizationId), "Organization not found.");

            // Parent department validation (must exist and be in same org/tenant)
            Department? parent = null;
            if (dto.ParentDepartmentId.HasValue)
            {
                if (dto.ParentDepartmentId.Value == 0)
                {
                    dto.ParentDepartmentId = null; // ignore 0 as "no parent"
                }
                else
                {
                    parent = await _context.Departments
                        .AsNoTracking()
                        .FirstOrDefaultAsync(d => d.Id == dto.ParentDepartmentId.Value);

                    if (parent == null)
                        ModelState.AddModelError(nameof(dto.ParentDepartmentId), "Parent department not found.");
                    else if (org != null && (parent.OrganizationId != org.Id))
                        ModelState.AddModelError(nameof(dto.ParentDepartmentId), "Parent must be in the same organization.");
                }
            }

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var department = new Department
            {
                OrganizationId = dto.OrganizationId,
                TenantId = org!.TenantId, // authoritative tenant from org
                DepartmentName = dto.DepartmentName,
                DepartmentHead = dto.DepartmentHead, // can be null; model allows it
                InitialEmployeeCount = dto.InitialEmployeeCount,
                ParentDepartmentId = dto.ParentDepartmentId
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            // Return the created resource with the generated Id
            var created = new DepartmentDto
            {
                Id = department.Id,
                OrganizationId = department.OrganizationId,
                DepartmentName = department.DepartmentName,
                DepartmentHead = department.DepartmentHead ?? string.Empty,
                InitialEmployeeCount = department.InitialEmployeeCount,
                ParentDepartmentId = department.ParentDepartmentId
            };

            return CreatedAtAction(nameof(GetDepartmentById), new { id = department.Id }, created);
        }

        // PUT: api/departments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, DepartmentDto dto)
        {
            if (id != dto.Id) return BadRequest("Department ID mismatch.");
            if (string.IsNullOrWhiteSpace(dto.DepartmentName))
                ModelState.AddModelError(nameof(dto.DepartmentName), "Department name is required.");

            var existing = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
            if (existing == null) return NotFound();

            // Get (possibly new) organization and derive tenant
            var org = await _context.Organizations
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == dto.OrganizationId);

            if (org == null)
                ModelState.AddModelError(nameof(dto.OrganizationId), "Organization not found.");

            // Prevent self-parenting
            if (dto.ParentDepartmentId.HasValue && dto.ParentDepartmentId.Value == id)
                ModelState.AddModelError(nameof(dto.ParentDepartmentId), "A department cannot be its own parent.");

            // Validate parent department (same org/tenant)
            if (dto.ParentDepartmentId.HasValue)
            {
                var parent = await _context.Departments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == dto.ParentDepartmentId.Value);

                if (parent == null)
                    ModelState.AddModelError(nameof(dto.ParentDepartmentId), "Parent department not found.");
                else if (org != null && parent.OrganizationId != org.Id)
                    ModelState.AddModelError(nameof(dto.ParentDepartmentId), "Parent must be in the same organization.");
            }

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // Apply updates
            existing.OrganizationId = dto.OrganizationId;
            existing.TenantId = org!.TenantId; // keep tenant consistent with org
            existing.DepartmentName = dto.DepartmentName;
            existing.DepartmentHead = dto.DepartmentHead; // nullable ok
            existing.InitialEmployeeCount = dto.InitialEmployeeCount;
            existing.ParentDepartmentId = dto.ParentDepartmentId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/departments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DepartmentExists(int id) =>
            _context.Departments.Any(d => d.Id == id);
    }
}
