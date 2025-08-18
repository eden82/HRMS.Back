using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS.Backend.Models;
using HRMS.Backend.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using HRMS.Backend.DTOs;

namespace HRMS.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<List<EmployeeDto>>> GetEmployees()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .ToListAsync();

            var employeeDtos = employees.Select(employee => new EmployeeDto
            {
                EmployeeID = employee.EmployeeID,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DateOfBirth = employee.DateOfBirth,
                Gender = employee.Gender,
                Nationality = employee.Nationality,
                MaritalStatus = employee.MaritalStatus,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Address = employee.Address,
                EmergencyContact = employee.EmergencyContact,
                JobTitle = employee.JobTitle,
                EmploymentType = employee.EmploymentType,
                Manager = employee.Manager,
                JoiningDate = employee.JoiningDate,
                Role = employee.Role,
                Salary = employee.Salary,
                Currency = employee.Currency,
                PaymentMethod = employee.PaymentMethod,
                BankAccountNumber = employee.BankAccountNumber,
                TaxIdentificationNumber = employee.TaxIdentificationNumber,
                BenefitsEnrollment = employee.BenefitsEnrollment,
                PassportNumber = employee.PassportNumber,
                ResumePath = employee.ResumePath,
                ContractFilePath = employee.ContractFilePath,
                CertificationPath = employee.CertificationPath,
                Username = employee.Username,
                WorkLocation = employee.WorkLocation,
                ShiftDetails = employee.ShiftDetails,
                DepartmentName = employee.Department?.DepartmentName,
                CreatedAt = employee.CreatedAt,
                UpdatedAt = employee.UpdatedAt
            }).ToList();

            return Ok(employeeDtos);
        }

        // GET: api/employees/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (employee == null)
                return NotFound();

            var employeeDto = new EmployeeDto
            {
                EmployeeID = employee.EmployeeID,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DateOfBirth = employee.DateOfBirth,
                Gender = employee.Gender,
                Nationality = employee.Nationality,
                MaritalStatus = employee.MaritalStatus,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Address = employee.Address,
                EmergencyContact = employee.EmergencyContact,
                JobTitle = employee.JobTitle,
                EmploymentType = employee.EmploymentType,
                Manager = employee.Manager,
                JoiningDate = employee.JoiningDate,
                Role = employee.Role,
                Salary = employee.Salary,
                Currency = employee.Currency,
                PaymentMethod = employee.PaymentMethod,
                BankAccountNumber = employee.BankAccountNumber,
                TaxIdentificationNumber = employee.TaxIdentificationNumber,
                BenefitsEnrollment = employee.BenefitsEnrollment,
                PassportNumber = employee.PassportNumber,
                ResumePath = employee.ResumePath,
                ContractFilePath = employee.ContractFilePath,
                CertificationPath = employee.CertificationPath,
                Username = employee.Username,
                WorkLocation = employee.WorkLocation,
                ShiftDetails = employee.ShiftDetails,
                DepartmentName = employee.Department?.DepartmentName,
                CreatedAt = employee.CreatedAt,
                UpdatedAt = employee.UpdatedAt
            };

            return Ok(employeeDto);
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            employee.CreatedAt = DateTime.UtcNow;
            employee.UpdatedAt = DateTime.UtcNow;

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Map to DTO
            var employeeDto = new EmployeeDto
            {
                EmployeeID = employee.EmployeeID,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DateOfBirth = employee.DateOfBirth,
                DepartmentName = employee.Department?.DepartmentName,
                CreatedAt = employee.CreatedAt,
                UpdatedAt = employee.UpdatedAt
            };

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeID }, employeeDto);
        }

        // PUT: api/employees/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (id != employee.EmployeeID)
                return BadRequest();

            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null)
                return NotFound();

            // Update fields
            _context.Entry(existingEmployee).CurrentValues.SetValues(employee);
            existingEmployee.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/employees/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound();

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
