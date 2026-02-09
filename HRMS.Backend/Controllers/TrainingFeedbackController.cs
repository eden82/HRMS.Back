using HRMS.Backend.Data;
using HRMS.Backend.DTOs;
using HRMS.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Backend.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TrainingFeedbackController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TrainingFeedbackController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/training-feedback/user/{userId}
        [HttpPost("user/{userId}")]
        public async Task<IActionResult> Create(Guid userId, CreateTrainingFeedbackDto dto)
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


            //  Get enrollment
            var enrollment = await _context.TrainingEnrollments
                .Include(e => e.TrainingProgram)
                .FirstOrDefaultAsync(e => e.Id == dto.EnrollmentId);

            if (enrollment == null)
                return NotFound("Enrollment not found.");

            //  Validate program match
            if (enrollment.ProgramId != dto.ProgramId)
                return BadRequest("Enrollment does not belong to this program.");

            // Employee must be the enrolled employee
            if (enrollment.EmployeeId != employeeId)
                return BadRequest("This user is not enrolled for this program.");


            //  Tenant / Organization validation
            if (user.TenantId != enrollment.TenantId)
                return BadRequest("Tenant mismatch.");

            // Organization validation
            if (enrollment.OrganizationId == null && user.OrganizationId != null)
                return BadRequest("Organization mismatch.");

            if (enrollment.OrganizationId != null &&
                user.OrganizationId != enrollment.OrganizationId)
                return BadRequest("Organization mismatch.");

            if (user.TenantId == null)
                return BadRequest("User does not belong to a tenant.");

            //  Prevent duplicate feedback
            var exists = await _context.TrainingFeedbacks
                .AnyAsync(f => f.EnrollmentId == dto.EnrollmentId);

            if (exists)
                return BadRequest("Feedback already submitted for this enrollment.");

            //  Create feedback
            var feedback = new TrainingFeedback
            {
                ProgramId = dto.ProgramId,
                EnrollmentId = dto.EnrollmentId,
                EmployeeId = employeeId,
                TenantId = user.TenantId.Value,
                OrganizationId = user.OrganizationId,
                Feedback = dto.Feedback
            };

            _context.TrainingFeedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Feedback submitted successfully.",
                feedback.Id
            });
        }
    }
}
