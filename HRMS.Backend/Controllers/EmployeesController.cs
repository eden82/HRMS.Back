using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Backend.Data;
using HRMS.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAll()
        {
            var list = await _context.Employees
                .AsNoTracking()
                .ToListAsync();

            return Ok(list);
        }

        // GET: /api/employees/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Employee>> GetById(Guid id)
        {
            var emp = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (emp == null) return NotFound();
            return Ok(emp);
        }

        // POST: /api/employees
        [HttpPost]
        public async Task<ActionResult<Employee>> Create([FromBody] Employee input)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // Ensure PK
            if (input.EmployeeID == Guid.Empty)
                input.EmployeeID = Guid.NewGuid();

            // --- Basic uniqueness guards (per tenant) ---
            // Email
            if (!string.IsNullOrWhiteSpace(input.Email))
            {
                var emailClash = await _context.Employees
                    .AnyAsync(e => e.TenantId == input.TenantId && e.Email == input.Email);
                if (emailClash)
                    return Conflict(new { message = "Email already exists for this tenant." });
            }

            // EmployeeCode: generate if empty; enforce uniqueness per tenant
            if (string.IsNullOrWhiteSpace(input.EmployeeCode))
            {
                input.EmployeeCode = await GenerateUniqueEmployeeCodeAsync(input.TenantId, input.FirstName, input.LastName);
            }
            else
            {
                var codeClash = await _context.Employees
                    .AnyAsync(e => e.TenantId == input.TenantId && e.EmployeeCode == input.EmployeeCode);
                if (codeClash)
                    return Conflict(new { message = "Employee code already exists for this tenant." });
            }

            _context.Employees.Add(input);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = input.EmployeeID }, input);
        }

        // PUT: /api/employees/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Employee body)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var emp = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeID == id);
            if (emp == null) return NotFound();

            // If tenant/org/department/role changed, keep your existing business rules
            emp.TenantId = body.TenantId;
            emp.OrganizationId = body.OrganizationId;
            emp.DepartmentId = body.DepartmentId;
            emp.RoleId = body.RoleId;

            // Personal / contact
            emp.FirstName = body.FirstName;
            emp.LastName = body.LastName;
            emp.Email = body.Email;
            emp.PhoneNumber = body.PhoneNumber;
            emp.EmergencyContactName = body.EmergencyContactName;
            emp.EmergencyContactNumber = body.EmergencyContactNumber;
            emp.Gender = body.Gender;
            emp.Nationality = body.Nationality;
            emp.MaritalStatus = body.MaritalStatus;
            emp.Address = body.Address;
            emp.DateOfBirth = body.DateOfBirth;

            // Job
            emp.JobTitle = body.JobTitle;
            emp.EmployeeEducationStatus = body.EmployeeEducationStatus;
            emp.EmploymentType = body.EmploymentType;
            emp.PhotoUrl = body.PhotoUrl;
            emp.JoiningDate = body.JoiningDate;

            // Codes – enforce uniqueness per tenant if changed
            if (!string.Equals(emp.EmployeeCode, body.EmployeeCode, StringComparison.Ordinal))
            {
                var newCode = body.EmployeeCode;
                if (string.IsNullOrWhiteSpace(newCode))
                {
                    newCode = await GenerateUniqueEmployeeCodeAsync(emp.TenantId, emp.FirstName, emp.LastName);
                }
                else
                {
                    var clash = await _context.Employees.AnyAsync(e =>
                        e.TenantId == emp.TenantId &&
                        e.EmployeeCode == newCode &&
                        e.EmployeeID != emp.EmployeeID);
                    if (clash)
                        return Conflict(new { message = "Employee code already exists for this tenant." });
                }
                emp.EmployeeCode = newCode;
            }

            // Other blobs/json
            emp.BankDetails = body.BankDetails;
            emp.CustomFields = body.CustomFields;

            // Timestamps
            emp.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: /api/employees/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var emp = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeID == id);
            if (emp == null) return NotFound();

            _context.Employees.Remove(emp);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // --- Helpers ---
        private async Task<string> GenerateUniqueEmployeeCodeAsync(Guid tenantId, string? first, string? last)
        {
            // base: FIRSTLAST (letters only), fallback EMP
            var baseRaw = new string($"{first}{last}".Where(char.IsLetter).ToArray());
            if (string.IsNullOrWhiteSpace(baseRaw)) baseRaw = "EMP";
            baseRaw = baseRaw.ToUpperInvariant();
            if (baseRaw.Length > 6) baseRaw = baseRaw.Substring(0, 6);

            // try until unique (Tenant-scoped)
            string candidate;
            var rnd = new Random();
            do
            {
                var suffix = rnd.Next(100, 1000); // 100-999
                candidate = $"{baseRaw}-{suffix}";
            } while (await _context.Employees.AnyAsync(e => e.TenantId == tenantId && e.EmployeeCode == candidate));

            return candidate;
        }
    }
}
