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
        var departments = await _context.Departments.ToListAsync();

        var departmentDtos = departments.Select(d => new DepartmentDto
        {
            Id = d.Id,
            OrganizationId = d.OrganizationId,
            DepartmentName = d.DepartmentName,
            DepartmentHead = d.DepartmentHead,
            InitialEmployeeCount = d.InitialEmployeeCount,
            ParentDepartmentId = d.ParentDepartmentId
        }).ToList();

        return Ok(departmentDtos);
    }


        // GET: api/departments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDto>> GetDepartmentById(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();

            var dto = new DepartmentDto
            {
                Id = department.Id,
                OrganizationId = department.OrganizationId,
                DepartmentName = department.DepartmentName,
                DepartmentHead = department.DepartmentHead,
                InitialEmployeeCount = department.InitialEmployeeCount,
                ParentDepartmentId = department.ParentDepartmentId
            };

            return Ok(dto);
        }


        // POST: api/departments
        [HttpPost]
        public async Task<ActionResult<DepartmentDto>> CreateDepartment(DepartmentDto dto)
        {
            var department = new Department
            {
                OrganizationId = dto.OrganizationId,
                DepartmentName = dto.DepartmentName,
                DepartmentHead = dto.DepartmentHead,
                InitialEmployeeCount = dto.InitialEmployeeCount,
                ParentDepartmentId = dto.ParentDepartmentId
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            // Use nameof to avoid typos
            return CreatedAtAction(nameof(GetDepartmentById), new { id = department.Id }, dto);
        }


        // PUT: api/departments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, DepartmentDto dto)
        {
            if (id != dto.Id) return BadRequest("Department ID mismatch");

            var existing = await _context.Departments.FindAsync(id);
            if (existing == null) return NotFound();

            existing.OrganizationId = dto.OrganizationId;
            existing.DepartmentName = dto.DepartmentName;
            existing.DepartmentHead = dto.DepartmentHead;
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

            if (department == null)
                return NotFound();

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(d => d.Id == id);
        }
    }
}
