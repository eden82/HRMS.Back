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
    [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
    public class ShortlistController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ShortlistController(AppDbContext context)
        {
            _context = context;
        }

        // Get all shortlists
        [HttpGet]
        public async Task<IActionResult> GetAllShortlists()
        {
            var shortlists = await _context.Shortlists
                .Select(s => new
                {
                    s.ShortlistID,
                    s.position,
                    s.Name,
                    s.Status,
                    s.ShortlistedOn
                })
                .ToListAsync();

            return Ok(shortlists);
        }

        // Move applicant to shortlist & delete applicant
        [HttpPost("move/{applicantId}")]
        public async Task<IActionResult> MoveApplicantToShortlist(Guid applicantId)
        {
            // Fetch the applicant
            var applicant = await _context.Applicants.FirstOrDefaultAsync(a => a.Id == applicantId);
            if (applicant == null)
                return NotFound(new { message = "Applicant not found." });

            if (!applicant.JobId.HasValue)
                return BadRequest(new { message = "Applicant must have a JobID." });

            // Check if already in shortlist to avoid duplicates
            var alreadyShortlisted = await _context.Shortlists
             .AnyAsync(s => !string.IsNullOrEmpty(s.Email) &&
                            !string.IsNullOrEmpty(applicant.Email) &&
                            s.Email.ToLower() == applicant.Email.ToLower() &&
                            s.JobID == applicant.JobId.Value);


            if (alreadyShortlisted)
                return BadRequest(new { message = "Applicant is already shortlisted for this job." });

            // Create shortlist entry
            var shortlist = new Shortlist
            {
                JobID = applicant.JobId.Value,
                Name = applicant.Name,
                Email = applicant.Email,
                Phone = applicant.Phone,
                ResumeUrl = applicant.ResumeUrl,  // preserves file URL
                position = applicant.position,
                Status = "Shortlist",
                ShortlistedOn = DateTime.UtcNow
            };

            // Add to shortlist and remove from applicants
            _context.Shortlists.Add(shortlist);
            _context.Applicants.Remove(applicant);

            // Save changes
            await _context.SaveChangesAsync();

            // Return JSON same as GetAllShortlists
            var result = new
            {
                shortlist.ShortlistID,
                shortlist.position,
                shortlist.Name,
                shortlist.Status,
                shortlist.ShortlistedOn
            };

            return Ok(result);
        }


        // Get shortlists by JobID
        [HttpGet("job/{jobId}")]
        public async Task<IActionResult> GetShortlistsByJob(Guid jobId)
        {
            var shortlists = await _context.Shortlists
                .Where(s => s.JobID == jobId)
                .Select(s => new ShortlistDTO
                {
                    ShortlistID = s.ShortlistID,
                    JobID = s.JobID,
                    Name = s.Name,
                    Email = s.Email,
                    Phone = s.Phone,
                    ResumeUrl = s.ResumeUrl,
                    position = s.position,
                    Status = s.Status,
                    ShortlistedOn = s.ShortlistedOn
                })
                .ToListAsync();

            if (!shortlists.Any())
                return NotFound(new { message = "No shortlists found for this job." });

            return Ok(shortlists);
        }

        // DELETE: api/shortlist/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShortlist(Guid id)
        {
            var shortlist = await _context.Shortlists.FindAsync(id);
            if (shortlist == null)
                return NotFound(new { message = "Shortlist record not found." });

            _context.Shortlists.Remove(shortlist);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Shortlist record deleted successfully." });
        }



        // Get by tenant ID 
        [HttpGet("tenant/{tenantId}")]
        public async Task<IActionResult> GetShortlistedWithSummaryByTenant(Guid tenantId)
        {
            var today = DateTime.Today;
            string? jobTitle = null;

            // 1. Get shortlisted applicants filtered by tenant (null-safe)
            var shortlisted = await _context.Shortlists
                .Where(s => s.Job != null &&
                            s.Job.TenantID == tenantId &&
                            s.Job.OrganizationId == null)
                .Select(s => new
                {
                    s.ShortlistID,
                    s.JobID,
                    s.Name,
                    s.Email,
                    s.Phone,
                    s.ResumeUrl,
                    s.Notes,
                    jobTitle = s.position,
                    s.Status,
                    s.ShortlistedOn
                })
                .ToListAsync();

            // 2. Summary values filtered by tenant

            // Active Jobs
            var activeJobs = await _context.Jobs
                .Where(j => j.TenantID == tenantId &&
                            j.OrganizationId == null &&
                            j.ApplicationDeadline >= today)
                .CountAsync();

            // Total Applications (shortlists) for this tenant
            int totalApplications = await _context.Shortlists
                .Where(s => s.Job != null && s.Job.TenantID == tenantId)
                .CountAsync();

            // Interviews today (for this tenant)
            int interviewsToday = await _context.Interviews
                .Where(i => i.ScheduledDate != null &&
                            i.ScheduledDate.Value.Date == today &&
                            i.Shortlist != null &&
                            i.Shortlist.Job != null &&
                            i.Shortlist.Job.TenantID == tenantId)
                .CountAsync();

            // 3. Combined Response
            var response = new
            {
                summary = new
                {
                    activeJobs,
                    totalApplications,
                    interviewsToday
                },
                shortlistedApplicants = shortlisted
            };

            return Ok(response);
        }






    }
}
