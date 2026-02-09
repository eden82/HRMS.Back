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
    public class TrainingProgramsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TrainingProgramsController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/TrainingPrograms
        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainingProgramDto dto)
        {
            var tenantId = dto.TenantId; //  Use TenantId from request

            // Validate Instructor by Email
            var instructor = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Email.ToLower() == dto.InstructorEmail.ToLower()
                                       && e.TenantId == tenantId);

            if (instructor == null)
            {
                ModelState.AddModelError(nameof(dto.InstructorEmail), "Instructor not found for this tenant.");
                return BadRequest(ModelState);
            }

            // Organization & Tenant Rules
            if (dto.OrganizationId == null)
            {
                if (instructor.OrganizationId != null)
                    ModelState.AddModelError(nameof(dto.OrganizationId),
                        "Instructor must not belong to an organization for a tenant-level program.");

                if (instructor.TenantId != tenantId)
                    ModelState.AddModelError("TenantId",
                        "Instructor tenant does not match program tenant.");
            }
            else
            {
                if (instructor.OrganizationId == null)
                    ModelState.AddModelError(nameof(dto.OrganizationId),
                        "Instructor must belong to an organization for an organization-level program.");
                else if (instructor.OrganizationId != dto.OrganizationId)
                    ModelState.AddModelError(nameof(dto.OrganizationId),
                        "Instructor organization does not match program organization.");

                if (instructor.TenantId != tenantId)
                    ModelState.AddModelError("TenantId",
                        "Instructor tenant does not match program tenant.");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Create Program
            var program = new TrainingProgram
            {
                TenantId = tenantId,
                OrganizationId = dto.OrganizationId,
                Title = dto.Title,
                Category = dto.Category,
                Level = dto.Level,
                DurationHours = dto.DurationHours,
                InstructorId = instructor.EmployeeID,
                MaxEnrollment = dto.MaxEnrollment,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Description = dto.Description
            };

            _context.TrainingPrograms.Add(program);
            await _context.SaveChangesAsync();

            return Ok(program.Id);
        }

        // GET: api/TrainingPrograms
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid tenantId) //  Accept tenantId from query
        {
            var programs = await _context.TrainingPrograms
                .Include(p => p.Instructor)
                .Where(x => x.TenantId == tenantId)
                .Select(x => new TrainingProgramDto
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    OrganizationId = x.OrganizationId,
                    Title = x.Title,
                    Category = x.Category,
                    Level = x.Level,
                    DurationHours = x.DurationHours,
                    InstructorId = x.InstructorId,
                    InstructorName = x.Instructor.FirstName + " " + x.Instructor.LastName,
                    MaxEnrollment = x.MaxEnrollment,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Description = x.Description
                })
                .ToListAsync();

            return Ok(programs);
        }

        // PUT: api/TrainingPrograms/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CreateTrainingProgramDto dto)
        {
            var tenantId = dto.TenantId;

            var program = await _context.TrainingPrograms
                .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);

            if (program == null)
                return NotFound();

            // Validate instructor
            var instructor = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Email.ToLower() == dto.InstructorEmail.ToLower()
                                       && e.TenantId == tenantId);

            if (instructor == null)
            {
                ModelState.AddModelError(nameof(dto.InstructorEmail), "Instructor not found for this tenant.");
                return BadRequest(ModelState);
            }

            program.Title = dto.Title;
            program.Category = dto.Category;
            program.Level = dto.Level;
            program.DurationHours = dto.DurationHours;
            program.InstructorId = instructor.EmployeeID;
            program.MaxEnrollment = dto.MaxEnrollment;
            program.StartDate = dto.StartDate;
            program.EndDate = dto.EndDate;
            program.Description = dto.Description;
            program.OrganizationId = dto.OrganizationId;

            await _context.SaveChangesAsync();
            return NoContent();
        }


        // GET: api/TrainingPrograms/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var program = await _context.TrainingPrograms
                .Include(p => p.Instructor)
                .Include(p => p.Enrollments)
                    .ThenInclude(e => e.Feedback)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (program == null)
                return NotFound(new { message = "Training program not found." });

            return Ok(new
            {
                program.Id,
                program.Title,
                program.Category,
                program.Level,
                program.DurationHours,
                Instructor = program.Instructor.FirstName + " " + program.Instructor.LastName,
                program.MaxEnrollment,
                program.StartDate,
                program.EndDate,
                program.Description,
                program.TenantId,
                program.OrganizationId,
                program.CreatedAt,
                program.UpdatedAt,

                Enrollments = program.Enrollments.Select(e => new
                {
                    e.Id,
                    e.EmployeeId,
                    e.EnrollmentNote,
                    e.EnrolledAt,
                    Feedback = e.Feedback != null ? new
                    {
                        e.Feedback.Id,
                        e.Feedback.Feedback,
                        e.Feedback.CreatedAt
                    } : null
                })
            });
        }


        [HttpGet("tenant/{tenantId}")]
        public async Task<IActionResult> GetByTenant(Guid tenantId)
        {
            var programs = await _context.TrainingPrograms
                .Where(p => p.TenantId == tenantId && p.OrganizationId == null)
                .Include(p => p.Instructor)
                .Include(p => p.Enrollments)
                    .ThenInclude(e => e.Feedback)
                .ToListAsync();

            if (!programs.Any())
                return NotFound(new { message = "No training programs found for this tenant." });

            var data = programs.Select(p => new
            {
                p.Id,
                p.Title,
                p.Category,
                p.Level,
                p.DurationHours,
                Instructor = p.Instructor.FirstName + " " + p.Instructor.LastName,
                p.MaxEnrollment,
                p.StartDate,
                p.EndDate,
                p.Description,
                p.CreatedAt,
                p.UpdatedAt,

                TotalEnrollments = p.Enrollments.Count,
                AverageProgress = p.Enrollments.Any() ? Math.Round(p.Enrollments.Average(e => e.Progress), 2) : 0
            });

            // Metrics
            var activePrograms = programs.Count(p => p.EndDate.Date >= DateTime.UtcNow.Date);
            var totalEnrollments = programs.Sum(p => p.Enrollments.Count);
            var completionRate = programs.SelectMany(p => p.Enrollments).Any()
                ? Math.Round(programs.SelectMany(p => p.Enrollments).Average(e => e.Progress), 2)
                : 0;

            return Ok(new
            {
                Message = "Tenant-level training programs retrieved successfully.",
                Data = data,
                Metrics = new
                {
                    ActivePrograms = activePrograms,
                    TotalEnrollments = totalEnrollments,
                    CompletionRate = completionRate
                }
            });
        }



        // GET : api/TrainingPrograms/user/{userId}/not-enrolled
        [HttpGet("user/{userId}/not-enrolled")]
        public async Task<IActionResult> GetNotEnrolledProgramsByUser(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest("UserId is required.");

            //  Get user
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found.");

            if (user.EmployeeId == null)
                return BadRequest("User is not linked to an employee.");

            var employeeId = user.EmployeeId.Value;
            var tenantId = user.TenantId;

            //  Get ProgramIds already enrolled by this employee
            var enrolledProgramIds = await _context.TrainingEnrollments
                .Where(e => e.EmployeeId == employeeId)
                .Select(e => e.ProgramId)
                .ToListAsync();

            //  Get programs under tenant NOT in enrolled list
            var programs = await _context.TrainingPrograms
                .Where(p =>
                    p.TenantId == tenantId &&
                    !enrolledProgramIds.Contains(p.Id)
                )
                .Include(p => p.Instructor)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            if (!programs.Any())
                return NotFound(new { message = "No available training programs found." });

            //  Response
            var result = programs.Select(p => new
            {
                ProgramId = p.Id,
                p.Title,
                p.Category,
                p.Level,
                Instructor = p.Instructor.FirstName + " " + p.Instructor.LastName,
                p.DurationHours,
                p.MaxEnrollment,
                p.StartDate,
                p.EndDate,
                p.Description,
                p.CreatedAt
            });

            return Ok(result);
        }


        // DELETE: api/TrainingPrograms/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Program Id is required.");

            var program = await _context.TrainingPrograms
                .Include(p => p.Enrollments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (program == null)
                return NotFound(new { message = "Training program not found." });

            //  Prevent deleting programs with enrollments
            if (program.Enrollments.Any())
                return BadRequest(new
                {
                    message = "Cannot delete training program with active enrollments."
                });

            _context.TrainingPrograms.Remove(program);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Training program deleted successfully.",
                programId = program.Id
            });
        }





    }
}
