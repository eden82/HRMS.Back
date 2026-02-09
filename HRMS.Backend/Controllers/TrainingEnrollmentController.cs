using HRMS.Backend.Data;
using HRMS.Backend.DTOs;
using HRMS.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS.Backend.Filters;

namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingEnrollmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly EmailService _emailService;

        public TrainingEnrollmentController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }


        // POST: api/training-enrollments
        [HttpPost]
        public async Task<IActionResult> Enroll(CreateTrainingEnrollmentDto dto)
        {
            //  Load Program
            var program = await _context.TrainingPrograms
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == dto.ProgramId);

            if (program == null)
                return NotFound("Training program not found.");

            //  Load Employee by Email
            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Email == dto.EmployeeEmail);

            if (employee == null)
                return NotFound("Employee not found with the given email.");

            //  Instructor check
            if (program.InstructorId == employee.EmployeeID)
                return BadRequest("Instructor cannot enroll in their own program.");

            //  Tenant validation
            if (dto.TenantId != employee.TenantId || dto.TenantId != program.TenantId)
                return BadRequest("Tenant mismatch detected.");

            //  Organization validation
            if (program.OrganizationId == null)
            {
                if (employee.OrganizationId != null)
                    return BadRequest("Tenant-level program. Employee must not belong to an organization.");
            }
            else
            {
                if (employee.OrganizationId == null || employee.OrganizationId != program.OrganizationId)
                    return BadRequest("Employee does not belong to the program's organization.");
            }

            //  MaxEnrollment check
            var currentCount = await _context.TrainingEnrollments
                .CountAsync(e => e.ProgramId == dto.ProgramId);

            if (currentCount >= program.MaxEnrollment)
                return BadRequest("Maximum enrollment limit reached.");

            //  Prevent duplicate enrollment
            var exists = await _context.TrainingEnrollments
                .AnyAsync(e => e.ProgramId == dto.ProgramId && e.EmployeeId == employee.EmployeeID);

            if (exists)
                return BadRequest("Employee is already enrolled in this program.");

            //  Create Enrollment
            var enrollment = new TrainingEnrollment
            {
                ProgramId = program.Id,
                EmployeeId = employee.EmployeeID,
                TenantId = dto.TenantId,
                OrganizationId = dto.OrganizationId,
                EnrollmentNote = dto.EnrollmentNote
            };

            _context.TrainingEnrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            //  Send notifications
            var tasks = new List<Task>();

            // Get instructor email from employee table
            if (dto.ManagerNotify && program.InstructorId != Guid.Empty)
            {
                var instructor = await _context.Employees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.EmployeeID == program.InstructorId);

                if (instructor != null && !string.IsNullOrWhiteSpace(instructor.Email))
                {
                    var employeeName = $"{employee.FirstName} {employee.LastName}";

                    tasks.Add(_emailService.SendEmailAsync(
                        instructor.Email,
                        "New Enrollment Notification",
                        $"Employee {employeeName} has been enrolled in your program '{program.Title}'."
                    ));
                }
            }

            // Employee notification
            if (dto.EmployeeNotify)
            {
                tasks.Add(_emailService.SendEmailAsync(
                    employee.Email,
                    "Enrollment Confirmation",
                    $"You have been successfully enrolled in the training program '{program.Title}'."
                ));
            }

            await Task.WhenAll(tasks);

            return Ok(new
            {
                message = "Employee enrolled successfully.",
                enrollmentId = enrollment.Id
            });
        }






        // POST: api/training-enrollments/by-user
        [HttpPost("by-user")]
        public async Task<IActionResult> EnrollByUser(CreateTrainingEnrollmentByUserDto dto)
        {
            //  Load Program
            var program = await _context.TrainingPrograms
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == dto.ProgramId);

            if (program == null)
                return NotFound("Training program not found.");

            // Load User and get EmployeeId
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == dto.UserId);

            if (user == null)
                return NotFound("User not found.");

            if (user.EmployeeId == null)
                return BadRequest("This user is not linked to an employee.");

            // Load Employee using EmployeeId from User
            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeID == user.EmployeeId);

            if (employee == null)
                return NotFound("Employee not found for the given user.");

            //  Tenant validation
            if (employee.TenantId != program.TenantId)
                return BadRequest("Employee and program belong to different tenants.");

            //  Organization validation
            if (program.OrganizationId == null)
            {
                if (employee.OrganizationId != null)
                    return BadRequest("Tenant-level program. Employee must not belong to an organization.");
            }
            else
            {
                if (employee.OrganizationId == null || employee.OrganizationId != program.OrganizationId)
                    return BadRequest("Employee does not belong to the program's organization.");
            }

            //  Max enrollment check
            var currentCount = await _context.TrainingEnrollments
                .CountAsync(e => e.ProgramId == dto.ProgramId);

            if (currentCount >= program.MaxEnrollment)
                return BadRequest("Maximum enrollment limit reached.");

            //  Prevent duplicate enrollment
            var exists = await _context.TrainingEnrollments
                .AnyAsync(e => e.ProgramId == dto.ProgramId && e.EmployeeId == employee.EmployeeID);

            if (exists)
                return BadRequest("Employee is already enrolled in this program.");

            //  Create Enrollment (EnrollmentNote is null)
            var enrollment = new TrainingEnrollment
            {
                ProgramId = program.Id,
                EmployeeId = employee.EmployeeID,
                TenantId = program.TenantId,
                OrganizationId = program.OrganizationId,
                EnrollmentNote = null
            };

            _context.TrainingEnrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Employee enrolled successfully.",
                enrollmentId = enrollment.Id
            });
        }




        // PUT: api/TrainingEnrollment/progress/by-user
        [HttpPut("progress/by-user")]
        public async Task<IActionResult> UpdateProgressByUser(UpdateEnrollmentProgressByUserDto dto)
        {
            //  Load user
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == dto.UserId);

            if (user == null)
                return NotFound("User not found.");

            if (user.EmployeeId == null)
                return BadRequest("User is not linked to an employee.");

            var employeeId = user.EmployeeId.Value;

            //  Load enrollment
            var enrollment = await _context.TrainingEnrollments
                .FirstOrDefaultAsync(e =>
                    e.ProgramId == dto.ProgramId &&
                    e.EmployeeId == employeeId);

            if (enrollment == null)
                return NotFound("Enrollment not found for this user.");

            //  Validate range
            if (dto.Progress < 0 || dto.Progress > 100)
                return BadRequest("Progress must be between 0 and 100.");

            enrollment.Progress = dto.Progress;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Progress updated successfully.",
                enrollmentId = enrollment.Id,
                progress = enrollment.Progress
            });
        }



        // GET: api/training-enrollments/tenant/{tenantId}
        [HttpGet("tenant/{tenantId}")]
        public async Task<IActionResult> GetByTenant(Guid tenantId)
        {
            var enrollments = await _context.TrainingEnrollments
                .Where(e =>
                    e.TenantId == tenantId &&
                    e.OrganizationId == null
                )
                .Include(e => e.TrainingProgram)
                .Include(e => e.Employee)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync();

            if (!enrollments.Any())
                return NotFound(new { message = "No tenant-level enrollments found." });

            // -------- DATA --------
            var data = enrollments.Select(e => new
            {
                e.Id,
                e.ProgramId,
                ProgramTitle = e.TrainingProgram.Title,
                e.EmployeeId,

                EmployeeName = e.Employee != null
                    ? e.Employee.FirstName + " " + e.Employee.LastName
                    : null,

                JobTitle = e.Employee?.JobTitle,


                e.Progress,
                e.EnrollmentNote,
                e.EnrolledAt
            });

            // -------- METRICS (SAME LOGIC AS TrainingPrograms) --------
            var activePrograms = enrollments
                .Select(e => e.TrainingProgram)
                .Distinct()
                .Count(p => p.EndDate.Date >= DateTime.UtcNow.Date);

            var totalEnrollments = enrollments.Count;

            var completionRate = enrollments.Any()
                ? Math.Round(enrollments.Average(e => e.Progress), 2)
                : 0;

            return Ok(new
            {
                Message = "Tenant-level training enrollments retrieved successfully.",
                Data = data,
                Metrics = new
                {
                    ActivePrograms = activePrograms,
                    TotalEnrollments = totalEnrollments,
                    CompletionRate = completionRate
                }
            });
        }


        // GET: api/training-enrollments/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetEnrollmentsByUser(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest("UserId is required.");

            // Get user
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found.");

            if (user.EmployeeId == null)
                return BadRequest("User is not linked to an employee.");

            var employeeId = user.EmployeeId.Value;

            // Get enrollments with program, instructor, and materials
            var enrollments = await _context.TrainingEnrollments
                .Where(e => e.EmployeeId == employeeId)
                .Include(e => e.TrainingProgram)
                    .ThenInclude(p => p.Instructor)
                .Include(e => e.TrainingProgram)
                    .ThenInclude(p => p.TrainingMaterials) // include training materials
                .AsNoTracking()
                .Select(e => new
                {
                    EnrollmentId = e.Id,
                    e.EnrolledAt,
                    e.Progress,

                    ProgramId = e.TrainingProgram.Id,
                    e.TrainingProgram.Title,
                    e.TrainingProgram.Category,
                    e.TrainingProgram.Level,
                    e.TrainingProgram.DurationHours,
                    e.TrainingProgram.StartDate,
                    e.TrainingProgram.EndDate,
                    e.TrainingProgram.Description,

                    InstructorName =
                        e.TrainingProgram.Instructor != null
                            ? e.TrainingProgram.Instructor.FirstName + " " + e.TrainingProgram.Instructor.LastName
                            : "Unknown",

                    InstructorJobTitle =
                        e.TrainingProgram.Instructor != null
                            ? e.TrainingProgram.Instructor.JobTitle
                            : null,

                    Materials = e.TrainingProgram.TrainingMaterials
                        .Select(m => new
                        {
                            m.Id,
                            m.DocumentOrVideoLink,
                            m.ContractFileName,
                            m.ContractFilePath,
                            m.CreatedAt
                        })
                        .ToList()
                })
                .ToListAsync();

            if (!enrollments.Any())
                return NotFound("No training enrollments found for this user.");

            return Ok(enrollments);
        }






    }
}