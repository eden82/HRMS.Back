using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS.Backend.Data;
using HRMS.Backend.Models;
using System;

namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/employee")]
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
                    Id = e.EmployeeID,
                    FirstName = e.FirstName,
                    MiddleName = e.MiddleName,
                    LastName = e.LastName,
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    DepartmentId = e.DepartmentId,
                    OrganizationId = e.OrganizationId,
                    TenantId = e.TenantId,
                    RoleId = e.RoleId
                })
                .ToListAsync();

            return Ok(list);
        }

        // GET: /api/employees/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(Guid id)
        {
            var e = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.EmployeeID == id);
            if (e is null) return NotFound();

            return Ok(new EmployeeDto
            {
                Id = e.EmployeeID,
                FirstName = e.FirstName,
                MiddleName = e.MiddleName,
                LastName = e.LastName,
                Email = e.Email,
                PhoneNumber = e.PhoneNumber,
                DepartmentId = e.DepartmentId,
                OrganizationId = e.OrganizationId,
                TenantId = e.TenantId,
                RoleId = e.RoleId
            });
        }

        // POST: /api/employees
        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] EmployeeCreateUpdateDto dto)
        {
            // Validate Department (derive org/tenant)
            var dept = await _context.Departments.AsNoTracking().FirstOrDefaultAsync(d => d.Id == dto.DepartmentId);
            if (dept is null) return BadRequest(new { message = "Invalid DepartmentId." });

            // Validate Role exists
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == dto.RoleId);
            if (!roleExists) return BadRequest(new { message = "Invalid RoleId." });

            var e = new Employee
            {
                EmployeeID = Guid.NewGuid(),
                DepartmentId = dto.DepartmentId,
                OrganizationId = dept.OrganizationId,
                TenantId = dept.TenantId,
                RoleId = dto.RoleId,

                FirstName = dto.FirstName.Trim(),
                MiddleName = dto.MiddleName.Trim(),
                LastName = dto.LastName.Trim(),

                Email = dto.Email.Trim(),
                PhoneNumber = dto.PhoneNumber.Trim(),
                EmergencyContactName = dto.EmergencyContactName.Trim(),
                EmergencyContactNumber = dto.EmergencyContactNumber.Trim(),

                Gender = dto.Gender.Trim(),
                Nationality = dto.Nationality.Trim(),
                MaritalStatus = dto.MaritalStatus.Trim(),
                Address = dto.Address.Trim(),

                DateOfBirth = dto.DateOfBirth,
                JobTitle = dto.JobTitle.Trim(),
                EmployeeCode = dto.EmployeeCode.Trim(),
                EmployeeEducationStatus = dto.EmployeeEducationStatus.Trim(),
                EmploymentType = dto.EmploymentType.Trim(),
                PhotoUrl = dto.PhotoUrl.Trim(),
                JoiningDate = dto.JoiningDate,

                BankDetails = dto.BankDetails,
                CustomFields = string.IsNullOrWhiteSpace(dto.CustomFields) ? "{}" : dto.CustomFields,

                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
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
                PhoneNumber = e.PhoneNumber,
                DepartmentId = e.DepartmentId,
                OrganizationId = e.OrganizationId,
                TenantId = e.TenantId,
                RoleId = e.RoleId
            };

            return CreatedAtAction(nameof(GetEmployee), new { id = created.Id }, created);
        }

        // PUT: /api/employees/{id}
        [HttpPut("{id:guid}")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] EmployeeCreateUpdateDto dto)
        {
            var e = await _context.Employees.FirstOrDefaultAsync(x => x.EmployeeID == id);
            if (e is null) return NotFound();

            var dept = await _context.Departments.AsNoTracking().FirstOrDefaultAsync(d => d.Id == dto.DepartmentId);
            if (dept is null) return BadRequest(new { message = "Invalid DepartmentId." });

            var roleExists = await _context.Roles.AnyAsync(r => r.Id == dto.RoleId);
            if (!roleExists) return BadRequest(new { message = "Invalid RoleId." });

            e.DepartmentId = dto.DepartmentId;
            e.OrganizationId = dept.OrganizationId;
            e.TenantId = dept.TenantId;
            e.RoleId = dto.RoleId;

            e.FirstName = dto.FirstName.Trim();
            e.MiddleName = dto.MiddleName.Trim();
            e.LastName = dto.LastName.Trim();

            e.Email = dto.Email.Trim();
            e.PhoneNumber = dto.PhoneNumber.Trim();
            e.EmergencyContactName = dto.EmergencyContactName.Trim();
            e.EmergencyContactNumber = dto.EmergencyContactNumber.Trim();

            e.Gender = dto.Gender.Trim();
            e.Nationality = dto.Nationality.Trim();
            e.MaritalStatus = dto.MaritalStatus.Trim();
            e.Address = dto.Address.Trim();

            e.DateOfBirth = dto.DateOfBirth;
            e.JobTitle = dto.JobTitle.Trim();
            e.EmployeeCode = dto.EmployeeCode.Trim();
            e.EmployeeEducationStatus = dto.EmployeeEducationStatus.Trim();
            e.EmploymentType = dto.EmploymentType.Trim();
            e.PhotoUrl = dto.PhotoUrl.Trim();
            e.JoiningDate = dto.JoiningDate;

            e.BankDetails = dto.BankDetails;
            e.CustomFields = string.IsNullOrWhiteSpace(dto.CustomFields) ? "{}" : dto.CustomFields;
            e.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: /api/employees/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            var e = await _context.Employees.FirstOrDefaultAsync(x => x.EmployeeID == id);
            if (e is null) return NotFound();

            _context.Employees.Remove(e);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    // ===== DTOs =====
    public sealed class EmployeeDto
    {
        public Guid Id { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid TenantId { get; set; }
        public Guid RoleId { get; set; }

        public string FirstName { get; set; } = "";
        public string MiddleName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
    }

    public sealed class EmployeeCreateUpdateDto
    {
        // Required FKs
        public Guid DepartmentId { get; set; }
        public Guid RoleId { get; set; }

        // Required fields (match model)
        public string FirstName { get; set; } = "";
        public string MiddleName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string EmergencyContactName { get; set; } = "";
        public string EmergencyContactNumber { get; set; } = "";
        public string Gender { get; set; } = "";
        public string Nationality { get; set; } = "";
        public string MaritalStatus { get; set; } = "";
        public string Address { get; set; } = "";

        public DateTime DateOfBirth { get; set; }

        public string JobTitle { get; set; } = "";
        public string EmployeeCode { get; set; } = "";
        public string EmployeeEducationStatus { get; set; } = "";
        public string EmploymentType { get; set; } = "";
        public string PhotoUrl { get; set; } = "";
        public DateTime JoiningDate { get; set; }

        public string BankDetails { get; set; } = "";
        public string CustomFields { get; set; } = "{}";
    }
}
