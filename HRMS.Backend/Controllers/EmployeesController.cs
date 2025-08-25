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
    [Route("api/[controller]")] // /api/employees
    [Route("api/employee")]     // /api/employee (alias)
    [Produces("application/json")]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EmployeesController(AppDbContext context) => _context = context;

        // GET: /api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var list = await _context.Employees
                .AsNoTracking()
                .Select(e => new EmployeeDto
                {
                    Id = e.EmployeeID,               // if your model uses Id, change to e.Id
                    FirstName = e.FirstName,
                    MiddleName = e.MiddleName,
                    LastName = e.LastName,
                    Email = e.Email,
                    DepartmentId = e.DepartmentId,
                    OrganizationId = e.OrganizationId,
                    TenantId = e.TenantId,
                    ManagerId = e.ManagerId
                })
                .ToListAsync();

            return Ok(list);
        }

        // GET: /api/employees/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            var e = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.EmployeeID == id); // if your model uses Id, change to x.Id

            if (e is null) return NotFound();

            var dto = new EmployeeDto
            {
                Id = e.EmployeeID,
                FirstName = e.FirstName,
                MiddleName = e.MiddleName,
                LastName = e.LastName,
                Email = e.Email,
                DepartmentId = e.DepartmentId,
                OrganizationId = e.OrganizationId,
                TenantId = e.TenantId,
                ManagerId = e.ManagerId
            };
            return Ok(dto);
        }

        // POST: /api/employees
        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] EmployeeCreateUpdateDto dto)
        {
            // Validate department
            var dept = await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == dto.DepartmentId);

            if (dept is null)
                return BadRequest(new { message = "Invalid DepartmentId." });

            // Validate manager (if sent) belongs to the same tenant
            if (dto.ManagerId.HasValue)
            {
                var mgr = await _context.Employees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.EmployeeID == dto.ManagerId.Value);
                if (mgr is null)
                    return BadRequest(new { message = "Manager not found." });
                if (mgr.TenantId != dept.TenantId)
                    return BadRequest(new { message = "Manager must belong to the same tenant as the department." });
            }

            var e = new Employee
            {
                DepartmentId = dto.DepartmentId,
                ManagerId = dto.ManagerId,

                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                LastName = dto.LastName,

                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                EmergencyContactName = dto.EmergencyContactName,
                EmergencyContactNumber = dto.EmergencyContactNumber,

                Gender = dto.Gender,
                Nationality = dto.Nationality,
                MaritalStatus = dto.MaritalStatus,
                Address = dto.Address,

                DateOfBirth = dto.DateOfBirth,

                JobTitle = dto.JobTitle,
                EmployeeCode = dto.EmployeeCode,
                EmployeeEducationStatus = dto.EmployeeEducationStatus,
                EmploymentType = dto.EmploymentType,
                PhotoUrl = dto.PhotoUrl,

                BankDetails = dto.BankDetails,
                CustomFields = dto.CustomFields,

                // derive from department
                OrganizationId = dept.OrganizationId,
                TenantId = dept.TenantId,

                CreatedAt = System.DateTime.UtcNow,
                UpdatedAt = System.DateTime.UtcNow
            };

            _context.Employees.Add(e);
            await _context.SaveChangesAsync();

            var created = new EmployeeDto
            {
                Id = e.EmployeeID,
                FirstName = e.FirstName,
                MiddleName = e.MiddleName,
                LastName = e.LastName,
                Email = e.Email,
                DepartmentId = e.DepartmentId,
                OrganizationId = e.OrganizationId,
                TenantId = e.TenantId,
                ManagerId = e.ManagerId
            };

            return CreatedAtAction(nameof(GetEmployee), new { id = created.Id }, created);
        }

        // PUT: /api/employees/5
        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeCreateUpdateDto dto)
        {
            var e = await _context.Employees.FirstOrDefaultAsync(x => x.EmployeeID == id);
            if (e is null) return NotFound();

            var dept = await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == dto.DepartmentId);
            if (dept is null)
                return BadRequest(new { message = "Invalid DepartmentId." });

            if (dto.ManagerId.HasValue)
            {
                var mgr = await _context.Employees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.EmployeeID == dto.ManagerId.Value);
                if (mgr is null)
                    return BadRequest(new { message = "Manager not found." });
                if (mgr.TenantId != dept.TenantId)
                    return BadRequest(new { message = "Manager must belong to the same tenant as the department." });
            }

            // map fields
            e.DepartmentId = dto.DepartmentId;
            e.ManagerId = dto.ManagerId;

            e.FirstName = dto.FirstName;
            e.MiddleName = dto.MiddleName;
            e.LastName = dto.LastName;

            e.Email = dto.Email;
            e.PhoneNumber = dto.PhoneNumber;
            e.EmergencyContactName = dto.EmergencyContactName;
            e.EmergencyContactNumber = dto.EmergencyContactNumber;

            e.Gender = dto.Gender;
            e.Nationality = dto.Nationality;
            e.MaritalStatus = dto.MaritalStatus;
            e.Address = dto.Address;

            e.DateOfBirth = dto.DateOfBirth;

            e.JobTitle = dto.JobTitle;
            e.EmployeeCode = dto.EmployeeCode;
            e.EmployeeEducationStatus = dto.EmployeeEducationStatus;
            e.EmploymentType = dto.EmploymentType;
            e.PhotoUrl = dto.PhotoUrl;

            e.BankDetails = dto.BankDetails;
            e.CustomFields = dto.CustomFields;

            // re-derive from department
            e.OrganizationId = dept.OrganizationId;
            e.TenantId = dept.TenantId;

            e.UpdatedAt = System.DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: /api/employees/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var e = await _context.Employees.FirstOrDefaultAsync(x => x.EmployeeID == id);
            if (e is null) return NotFound();

            _context.Employees.Remove(e);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    // ===== DTOs (kept simple, no nullable warnings) =====
    public sealed class EmployeeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = "";
        public string? Email { get; set; }
        public int DepartmentId { get; set; }
        public int OrganizationId { get; set; }
        public int TenantId { get; set; }
        public int? ManagerId { get; set; }
    }

    public sealed class EmployeeCreateUpdateDto
    {
        public int DepartmentId { get; set; }
        public int? ManagerId { get; set; }

        public string FirstName { get; set; } = "";
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = "";

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactNumber { get; set; }

        public string? Gender { get; set; }
        public string? Nationality { get; set; }
        public string? MaritalStatus { get; set; }
        public string? Address { get; set; }

        public System.DateTime? DateOfBirth { get; set; }

        public string? JobTitle { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeEducationStatus { get; set; }
        public string? EmploymentType { get; set; }
        public string? PhotoUrl { get; set; }

        public string? BankDetails { get; set; }
        public string? CustomFields { get; set; }
    }
}
