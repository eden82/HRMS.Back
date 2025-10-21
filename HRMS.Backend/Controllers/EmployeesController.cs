using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HRMS.Backend.Data;
using HRMS.Backend.DTOs;
using HRMS.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS.Backend.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/employees")]
    [Produces("application/json")]
    [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;


        public EmployeesController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // POST: api/employees
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] EmployeeCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // Basic FK existence checks
            var tenantExists = await _context.Tenants.AnyAsync(t => t.Id == dto.TenantId);
            if (!tenantExists) return BadRequest($"Tenant {dto.TenantId} not found.");

            Organization? org = null;

            if (dto.OrganizationId != null)
            {
                org = await _context.Organizations.AsNoTracking()
                    .FirstOrDefaultAsync(o => o.Id == dto.OrganizationId && o.TenantId == dto.TenantId);

                if (org is null)
                    return BadRequest("Organization not found in the specified tenant.");
            }


            if (dto.DepartmentId.HasValue)
            {
                var deptOk = await _context.Departments.AnyAsync(d =>
                    d.Id == dto.DepartmentId.Value &&
                    d.OrganizationId == dto.OrganizationId &&
                    d.TenantId == dto.TenantId);
                if (!deptOk) return BadRequest("Department not found in the specified organization/tenant.");
            }

            // Uniqueness checks (per tenant)
            var emailClash = await _context.Employees.AnyAsync(e => e.TenantId == dto.TenantId && e.Email == dto.Email);
            if (emailClash) return Conflict(new { message = "Email already exists in tenant." });


            // EmployeeCode: generate if null/empty; ensure uniqueness per tenant
            var employeeCode = string.IsNullOrWhiteSpace(dto.EmployeeCode)
                ? await GenerateUniqueEmployeeCodeAsync(dto.TenantId, dto.FirstName, dto.LastName)
                : dto.EmployeeCode!.Trim().ToUpperInvariant();

            if (!string.IsNullOrWhiteSpace(dto.EmployeeCode))
            {
                var codeClash = await _context.Employees.AnyAsync(e => e.TenantId == dto.TenantId && e.EmployeeCode == employeeCode);
                if (codeClash) return Conflict(new { message = "Employee code already exists in tenant." });
            }



            string? photoUrl = null;
            string? resumeUrl = null;
            string? contractUrl = null;

            // Photo
            if (dto.Photo != null)
            {

                var rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadsFolderPhoto = Path.Combine(rootPath, "uploads", "employees" , "Photo");


                if (!Directory.Exists(uploadsFolderPhoto)) Directory.CreateDirectory(uploadsFolderPhoto);

                var fileNamePhoto = Guid.NewGuid().ToString() + Path.GetExtension(dto.Photo.FileName);
                var filePathPhoto = Path.Combine(uploadsFolderPhoto, fileNamePhoto);

                using (var stream = new FileStream(filePathPhoto, FileMode.Create))
                    await dto.Photo.CopyToAsync(stream);

                photoUrl = $"/uploads/employees/{fileNamePhoto}";
            }

            // Resume
            if (dto.ResumeFile != null)
            {


                var rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadsFolderResume = Path.Combine(rootPath, "uploads", "employees" , "ResumeFile");

                if (!Directory.Exists(uploadsFolderResume)) Directory.CreateDirectory(uploadsFolderResume);

                var fileNameResume = Guid.NewGuid().ToString() + Path.GetExtension(dto.ResumeFile.FileName);
                var filePathResume = Path.Combine(uploadsFolderResume, fileNameResume);

                using (var stream = new FileStream(filePathResume, FileMode.Create))
                    await dto.ResumeFile.CopyToAsync(stream);

                resumeUrl = $"/uploads/employees/resumes/{fileNameResume}";
            }

            // Contract
            if (dto.ContractFile != null)
            {
                var rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadsFolderContract = Path.Combine(rootPath, "uploads", "employees" , "ContractFile");


                if (!Directory.Exists(uploadsFolderContract)) Directory.CreateDirectory(uploadsFolderContract);

                var fileNameContract = Guid.NewGuid().ToString() + Path.GetExtension(dto.ContractFile.FileName);
                var filePathContract = Path.Combine(uploadsFolderContract, fileNameContract);

                using (var stream = new FileStream(filePathContract, FileMode.Create))
                    await dto.ContractFile.CopyToAsync(stream);

                contractUrl = $"/uploads/employees/contracts/{fileNameContract}";
            }



            // Certification

            string? certificationUrl = null;

            if (dto.CertificationFile != null)
            {

                var rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadsFolderCertification = Path.Combine(rootPath, "uploads", "employees", "certifications");

                if (!Directory.Exists(uploadsFolderCertification)) Directory.CreateDirectory(uploadsFolderCertification);

                var fileNameCertification = Guid.NewGuid().ToString() + Path.GetExtension(dto.CertificationFile.FileName);
                var filePathCertification = Path.Combine(uploadsFolderCertification, fileNameCertification);

                using (var stream = new FileStream(filePathCertification, FileMode.Create))
                    await dto.CertificationFile.CopyToAsync(stream);

                certificationUrl = $"/uploads/employees/certifications/{fileNameCertification}";
            }



            // Create entity
            var entity = new Employee
            {
                    EmployeeID = Guid.NewGuid(),
                    TenantId = dto.TenantId,
                    OrganizationId = dto.OrganizationId,
                    DepartmentId = dto.DepartmentId,      // nullable OK

                    FirstName = dto.FirstName.Trim(),
                    LastName = dto.LastName.Trim(),
                    DateOfBirth = dto.DateOfBirth,
                    Gender = dto.Gender.Trim(),
                    Nationality = dto.Nationality.Trim(),
                    MaritalStatus = dto.MaritalStatus.Trim(),

                    Email = dto.Email.Trim(),
                    PhoneNumber = dto.PhoneNumber.Trim(),
                    Address = dto.Address.Trim(),
                    EmergencyContactName = dto.EmergencyContactName.Trim(),
                    EmergencyContactNumber = dto.EmergencyContactNumber.Trim(),

                    JobTitle = dto.JobTitle.Trim(),
                    EmploymentType = dto.EmploymentType.Trim(),
                    EmployeeEducationStatus = dto.EmployeeEducationStatus.Trim(),


                    PhotoUrl = photoUrl ?? string.Empty,
                    Resume = resumeUrl ?? string.Empty,
                    ContractFile = contractUrl ?? string.Empty,
                    Certification = certificationUrl ?? string.Empty,


                // If JoiningDate is null, use current UTC date as default
                HireDate = dto.HireDate == default ? DateTime.UtcNow : dto.HireDate,

                    EmployeeCode = employeeCode,

         
                    BenefitsEnrollment = dto.BenefitsEnrollment,
                    ShiftDetails = dto.ShiftDetails,
                    LeaveCredit = dto.LeaveCredit,
                    Salary = dto.Salary,
                    Currency = dto.Currency,
                    PaymentMethod = dto.PaymentMethod,
                    BankAccountNumber = dto.BankAccountNumber,
                    TaxIdenitificationNumber = dto.TaxIdenitificationNumber,
                    PassportNumber = dto.PassportNumber,
                    WorkLocation = dto.WorkLocation,

                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
            };

            _context.Employees.Add(entity);
            await _context.SaveChangesAsync();

            // Fetch the created employee along with department and role details
            var createdEmployee = await _context.Employees
                .Where(e => e.EmployeeID == entity.EmployeeID)
                .Include(e => e.Department)  // Include Department if it exists
                .AsNoTracking()
                .Select(e => new
                {
                    e.EmployeeID,
                    EmployeeName = $"{e.FirstName} {e.LastName}",
                    Department = e.Department != null ? e.Department.DepartmentName : null, // Null if no department
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.EmployeeID }, createdEmployee);
        }


        // DELETE: api/employees/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var e = await _context.Employees.FindAsync(id);
            if (e == null) return NotFound();

            _context.Employees.Remove(e);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        // GET: api/employees/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var e = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.EmployeeID == id);
            if (e == null) return NotFound();
            return Ok(e); // or map to a read DTO if you prefer
        }

        // PUT: api/employees/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] EmployeeUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (id != dto.EmployeeId) return BadRequest("Employee ID mismatch.");

            var e = await _context.Employees.FirstOrDefaultAsync(x => x.EmployeeID == id);
            if (e is null) return NotFound();

            // FK existence checks (same as create)
            var org = await _context.Organizations.AsNoTracking()
                         .FirstOrDefaultAsync(o => o.Id == dto.OrganizationId && o.TenantId == dto.TenantId);
            if (org is null) return BadRequest("Organization not found in the specified tenant.");

            if (dto.DepartmentId.HasValue)
            {
                var deptOk = await _context.Departments.AnyAsync(d =>
                    d.Id == dto.DepartmentId.Value &&
                    d.OrganizationId == dto.OrganizationId &&
                    d.TenantId == dto.TenantId);
                if (!deptOk) return BadRequest("Department not found in the specified organization/tenant.");
            }




            // Uniqueness checks on change
            if (!string.Equals(e.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailClash = await _context.Employees.AnyAsync(x =>
                    x.TenantId == dto.TenantId && x.Email == dto.Email && x.EmployeeID != e.EmployeeID);
                if (emailClash) return Conflict(new { message = "Email already exists in tenant." });
            }

            // Handle file uploads (optional)
            string rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // PHOTO
            if (dto.Photo != null && dto.Photo.Length > 0)
            {
                var folder = Path.Combine(rootPath, "uploads", "employees", "Photo");
                Directory.CreateDirectory(folder);

                // Delete old file if exists
                if (!string.IsNullOrWhiteSpace(e.PhotoUrl))
                {
                    var oldFilePath = Path.Combine(rootPath, e.PhotoUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(dto.Photo.FileName);
                var filePath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await dto.Photo.CopyToAsync(stream);

                e.PhotoUrl = $"/uploads/employees/Photo/{fileName}";
            }

            // RESUME
            if (dto.ResumeFile != null && dto.ResumeFile.Length > 0)
            {
                var folder = Path.Combine(rootPath, "uploads", "employees", "ResumeFile");
                Directory.CreateDirectory(folder);

                if (!string.IsNullOrWhiteSpace(e.Resume))
                {
                    var oldFilePath = Path.Combine(rootPath, e.Resume.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(dto.ResumeFile.FileName);
                var filePath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await dto.ResumeFile.CopyToAsync(stream);

                e.Resume = $"/uploads/employees/ResumeFile/{fileName}";
            }

            // CONTRACT
            if (dto.ContractFile != null && dto.ContractFile.Length > 0)
            {
                var folder = Path.Combine(rootPath, "uploads", "employees", "ContractFile");
                Directory.CreateDirectory(folder);

                if (!string.IsNullOrWhiteSpace(e.ContractFile))
                {
                    var oldFilePath = Path.Combine(rootPath, e.ContractFile.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(dto.ContractFile.FileName);
                var filePath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await dto.ContractFile.CopyToAsync(stream);

                e.ContractFile = $"/uploads/employees/ContractFile/{fileName}";
            }

            // CERTIFICATION
            if (dto.CertificationFile != null && dto.CertificationFile.Length > 0)
            {
                var folder = Path.Combine(rootPath, "uploads", "employees", "certifications");
                Directory.CreateDirectory(folder);

                if (!string.IsNullOrWhiteSpace(e.Certification))
                {
                    var oldFilePath = Path.Combine(rootPath, e.Certification.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(dto.CertificationFile.FileName);
                var filePath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await dto.CertificationFile.CopyToAsync(stream);

                e.Certification = $"/uploads/employees/certifications/{fileName}";
            }



            string? newCode = dto.EmployeeCode;
            if (string.IsNullOrWhiteSpace(newCode))
            {
                // keep existing; if empty, generate
                if (string.IsNullOrWhiteSpace(e.EmployeeCode))
                    e.EmployeeCode = await GenerateUniqueEmployeeCodeAsync(dto.TenantId, dto.FirstName, dto.LastName);
            }
            else
            {
                newCode = newCode.Trim().ToUpperInvariant();
                if (!string.Equals(e.EmployeeCode, newCode, StringComparison.Ordinal))
                {
                    var codeClash = await _context.Employees.AnyAsync(x =>
                        x.TenantId == dto.TenantId && x.EmployeeCode == newCode && x.EmployeeID != e.EmployeeID);
                    if (codeClash) return Conflict(new { message = "Employee code already exists in tenant." });
                    e.EmployeeCode = newCode;
                }
            }

            // Apply changes
            e.TenantId = dto.TenantId;
            e.OrganizationId = dto.OrganizationId;
            e.DepartmentId = dto.DepartmentId;
            e.FirstName = dto.FirstName.Trim();
            e.LastName = dto.LastName.Trim();
            e.DateOfBirth = dto.DateOfBirth;
            e.Gender = dto.Gender.Trim();
            e.Nationality = dto.Nationality.Trim();
            e.MaritalStatus = dto.MaritalStatus.Trim();

            e.Email = dto.Email.Trim();
            e.PhoneNumber = dto.PhoneNumber.Trim();
            e.Address = dto.Address.Trim();
            e.EmergencyContactName = dto.EmergencyContactName.Trim();
            e.EmergencyContactNumber = dto.EmergencyContactNumber.Trim();

            e.JobTitle = dto.JobTitle.Trim();
            e.EmploymentType = dto.EmploymentType.Trim();
            e.EmployeeEducationStatus = dto.EmployeeEducationStatus.Trim();
            //e.PhotoUrl = dto.Photo.Trim();
            e.HireDate = dto.HireDate;

            e.BenefitsEnrollment = dto.BenefitsEnrollment;
            e.ShiftDetails = dto.ShiftDetails;
            e.Salary = dto.Salary;
            e.Currency = dto.Currency;
            e.PaymentMethod = dto.PaymentMethod;
            e.BankAccountNumber = dto.BankAccountNumber;
            e.TaxIdenitificationNumber = dto.TaxIdenitificationNumber;
            e.PassportNumber = dto.PassportNumber;
            //e.Resume = dto.Resume;
            //e.ContractFile = dto.ContractFile;
            e.WorkLocation = dto.WorkLocation;
            //e.Certification = dto.Certification;

            e.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET by department
        [HttpGet("by-department/{tenantId}/{departmentId}")]
        public async Task<ActionResult<object>> GetByDepartment(Guid tenantId, Guid departmentId)
        {
            if (tenantId == Guid.Empty || departmentId == Guid.Empty)
                return BadRequest("tenantId and departmentId are required.");

            // Get total employees in department
            var totalEmployeesInDepartment = await _context.Employees
                .AsNoTracking()
                .Where(e => e.TenantId == tenantId && e.DepartmentId == departmentId)
                .CountAsync();

            // Get count of new hires in the department for the current month
            var newHiresInDepartment = await _context.Employees
                .AsNoTracking()
                .Where(e => e.TenantId == tenantId && e.DepartmentId == departmentId && e.HireDate.Month == DateTime.UtcNow.Month && e.HireDate.Year == DateTime.UtcNow.Year)
                .CountAsync();

            var employees = await _context.Employees
                .AsNoTracking()
                .Where(e => e.TenantId == tenantId && e.DepartmentId == departmentId)
                .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
                .Select(e => new EmployeeListDto
                {
                    EmployeeID = e.EmployeeID,
                    TenantId = e.TenantId,
                    OrganizationId = e.OrganizationId,
                    DepartmentId = e.DepartmentId,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    EmployeeCode = e.EmployeeCode,
                    JobTitle = e.JobTitle
                })
                .ToListAsync();

            return Ok(new
            {
                TotalEmployeesInDepartment = totalEmployeesInDepartment,
                NewHiresThisMonth = newHiresInDepartment,
                Employees = employees
            });
        }

        //get employee by employee code
        [HttpGet("by-employee-code/{tenantId}/{employeeCode}")]
        public async Task<ActionResult<EmployeeDetailDto>> GetByEmployeeCode(Guid tenantId, string employeeCode)
        {
            if (tenantId == Guid.Empty || string.IsNullOrWhiteSpace(employeeCode))
                return BadRequest("tenantId and employeeCode are required.");

            var employee = await _context.Employees
                .AsNoTracking()
                .Where(e => e.TenantId == tenantId && e.EmployeeCode == employeeCode)
                .Select(e => new EmployeeDetailDto
                {
                    EmployeeID = e.EmployeeID,
                    TenantId = e.TenantId,
                    OrganizationId = e.OrganizationId,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    EmployeeCode = e.EmployeeCode,
                    JobTitle = e.JobTitle,
                    DepartmentId = e.DepartmentId
                })
                .FirstOrDefaultAsync();

            if (employee == null)
                return NotFound($"Employee with code {employeeCode} not found.");

            return Ok(employee);
        }
        //get total employees in a tenant
        [HttpGet("total-employees/{tenantId}")]
        public async Task<IActionResult> GetTotalEmployees(Guid tenantId)
        {
            if (tenantId == Guid.Empty)
                return BadRequest("tenantId is required.");

            var totalEmployees = await _context.Employees
                .AsNoTracking()
                .CountAsync(e => e.TenantId == tenantId);

            return Ok(new { count = totalEmployees });
        }
 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeListDto>>> GetAllEmployees()
        {
            var employees = await _context.Employees
                .AsNoTracking()
                .Select(e => new EmployeeListDto
                {
                    EmployeeID = e.EmployeeID,
                    TenantId = e.TenantId,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    EmployeeCode = e.EmployeeCode,
                    JobTitle = e.JobTitle
                })
                .ToListAsync();

            return Ok(employees);
        }
        [HttpGet("by-tenant/{tenantId}")]
        public async Task<ActionResult<IEnumerable<EmployeeListDto>>> GetByTenant(string tenantId)
        {

            if (!Guid.TryParse(tenantId, out var tenantGuid))
                return BadRequest("Invalid tenantId format.");

            // Simple query that only filters by TenantId - no OrganizationId required
            var employees = await _context.Employees
                .Where(e => e.TenantId == tenantGuid)  
                .Select(e => new EmployeeListDto
                {
                    EmployeeID = e.EmployeeID,

                    TenantId = e.TenantId,
                    OrganizationId = e.OrganizationId,  
                    DepartmentId = e.DepartmentId,      
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    EmployeeCode = e.EmployeeCode,
                    JobTitle = e.JobTitle,
                    OrganizationName = e.Organization != null ? e.Organization.Name : null,
                    DepartmentName = e.Department != null ? e.Department.DepartmentName : null
                })
                .ToListAsync();


            // Return empty array instead of 404
            return Ok(employees);
        }


        // GET: api/employees/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllEmployeesWithDetails()
        {
            var employees = await _context.Employees
                .AsNoTracking()
                .Include(e => e.Organization)
                .Include(e => e.Department)
                .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
                .Select(e => new
                {
                    e.EmployeeID,
                    e.TenantId,
                    e.OrganizationId,
                    OrganizationName = e.Organization != null ? e.Organization.Name : null,
                    e.DepartmentId,
                    DepartmentName = e.Department != null ? e.Department.DepartmentName : null,
                    FullName = $"{e.FirstName} {e.LastName}",
                    e.FirstName,
                    e.LastName,
                    e.Email,
                    e.EmployeeCode,
                    e.JobTitle,
                    e.HireDate,
                    e.Salary,
                    e.Currency,
                    e.EmploymentType
                })
                .ToListAsync();

            if (!employees.Any())
                return NotFound("No employees found.");

            return Ok(employees);
        }


        // Helper method for generating a unique employee code
        private async Task<string> GenerateUniqueEmployeeCodeAsync(Guid tenantId, string firstName, string lastName)
        {
            var basePart = new string($"{firstName}{lastName}".Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
            if (basePart.Length < 4) basePart = (basePart + "XXXX").Substring(0, 4);
            else basePart = basePart.Substring(0, Math.Min(6, basePart.Length));

            string candidate;
            var rnd = RandomNumberGenerator.Create();
            do
            {
                var bytes = new byte[2];
                rnd.GetBytes(bytes);
                var suffix = (BitConverter.ToUInt16(bytes, 0) % 900 + 100);
                candidate = $"{basePart}-{suffix}";
            }
            while (await _context.Employees.AnyAsync(e => e.TenantId == tenantId && e.EmployeeCode == candidate));

            return candidate;
        }
    }
}
