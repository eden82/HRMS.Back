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
    public class InterviewsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InterviewsController(AppDbContext context)
        {
            _context = context;
        }

        // Schedule (Create) Interview
        [HttpPost]
        public async Task<IActionResult> CreateInterview([FromBody] InterviewCreateDto dto)
        {
            //  VALIDATE ApplicantEmail + JobTitle
            if (string.IsNullOrWhiteSpace(dto.ApplicantEmail) || string.IsNullOrWhiteSpace(dto.JobTitle))
                return BadRequest(new { message = "ApplicantEmail and JobTitle are required." });


            //  Find Shortlist using applicant email + job title
            var shortlist = await _context.Shortlists
                .Include(s => s.Job)
                .Where(s => s.Email == dto.ApplicantEmail && s.Job.JobTitle == dto.JobTitle)
                .FirstOrDefaultAsync();



            if (shortlist == null)
                return NotFound(new { message = "No shortlist found for the given Email and Job Title." });

            Guid shortlistId = shortlist.ShortlistID;

            var existingInterview = await _context.Interviews
               .FirstOrDefaultAsync(i => i.ShortlistId == shortlistId);

            if (existingInterview != null)
            {
                return BadRequest(new { message = "Cannot schedule the same applicant twice." });
            }

            //  VALIDATE Interviewer Email (optional)
            Guid? interviewerId = null;

            if (!string.IsNullOrWhiteSpace(dto.InterviewerEmail))
            {
                var interviewer = await _context.Employees
                    .Where(e => e.Email == dto.InterviewerEmail)
                    .FirstOrDefaultAsync();

                if (interviewer == null)
                    return NotFound(new { message = "Interviewer with given email not found." });

                interviewerId = interviewer.EmployeeID;
            }

            //  VALIDATE ScheduledDate and ScheduledTime
            if (!DateTime.TryParse(dto.ScheduledDate, out DateTime date))
                return BadRequest(new { message = "Invalid ScheduledDate format. Use YYYY-MM-DD." });

            if (!TimeSpan.TryParse(dto.ScheduledTime, out TimeSpan time))
                return BadRequest(new { message = "Invalid ScheduledTime format. Use HH:mm:ss." });

            // VALIDATE date is not in the past
            if (date < DateTime.UtcNow.Date)
                return BadRequest(new { message = "ScheduledDate cannot be in the past." });

            //  Create Interview Object
            var interview = new Interview
            {
                Id = Guid.NewGuid(),

                ShortlistId = shortlistId,

                ScheduledDate = date,
                ScheduledTime = time,

                Duration = dto.Duration,

                LocationORMeetingUrl = dto.LocationOrMeetingUrl,
                InterviewerId = interviewerId,

                InterviewNote = dto.InterviewNote,
                Mode = dto.Mode,
                Status = "Scheduled",

                ScheduledOn = DateTime.UtcNow
            };

            //  Save to DB
            _context.Interviews.Add(interview);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Interview created successfully.",
                data = interview
            });
        }




        //  2. Edit Interview
        [HttpPut("{id}")]
        public async Task<IActionResult> EditInterview(Guid id, [FromBody] InterviewDTO dto)
        {
            var exists = await _context.Interviews.AnyAsync(i => i.Id == id);
            if (!exists)
                return NotFound(new { message = $"Interview with Id {id} not found" });

            var interview = await _context.Interviews.FindAsync(id);

            interview!.InterviewerId = dto.InterviewerId;
            interview.Mode = dto.Type;
            interview.ScheduledDate = dto.ScheduledDate;
            interview.LocationORMeetingUrl = dto.LocationORMeetingUrl;
            interview.InterviewNote = dto.InterviewNote;
            interview.ScheduledOn = DateTime.UtcNow;

            _context.Interviews.Update(interview);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Interview updated successfully." });
        }


        //  3. Delete Interview
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInterview(Guid id)
        {
            var interview = await _context.Interviews.FindAsync(id);
            if (interview == null)
                return NotFound(new { message = "Interview not found" });

            _context.Interviews.Remove(interview);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Interview deleted successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInterviews()
        {
            var interviews = await _context.Interviews
                .Include(i => i.Shortlist)
                    .ThenInclude(s => s.Job)
                .Include(i => i.Interviewer)
                .Select(i => new InterviewDTO
                {
                    Id = i.Id,
                    ShortlistId = i.ShortlistId,

                    // Applicant info comes from Shortlist
                    ApplicantName = i.Shortlist.Name,
                    ApplicantEmail = i.Shortlist.Email ?? string.Empty,
                    Position = i.Shortlist.position ?? string.Empty,
                    JobTitle = i.Shortlist.Job.JobTitle,

                    // Interviewer info (NO null propagation)
                    InterviewerId = i.InterviewerId,
                    InterviewerName = i.InterviewerId != null
                        ? i.Interviewer.FirstName + " " + i.Interviewer.LastName
                        : "Not Assigned",

                    InterviewerEmail = i.InterviewerId != null
                        ? i.Interviewer.Email
                        : string.Empty,

                    // Interview details
                    Type = i.Mode,
                    Status = i.Status,
                    ScheduledDate = i.ScheduledDate,
                    ScheduledTime = i.ScheduledTime,
                    Duration = i.Duration,
                    LocationORMeetingUrl = i.LocationORMeetingUrl,
                    InterviewNote = i.InterviewNote
                })
                .ToListAsync();

            return Ok(new
            {
                message = "All interviews fetched successfully.",
                data = interviews
            });
        }


        ////  4. Get All Interviews
        //[HttpGet]
        //public async Task<IActionResult> GetAllInterviews()
        //{
        //    var interviews = await _context.Interviews
        //        .Include(i => i.Applicant)
        //        .Include(i => i.Interviewer)
        //        .Select(i => new InterviewDTO
        //        {
        //            Id = i.Id,
        //            ApplicantId = i.ApplicantId,
        //            ApplicantName = i.Applicant.Name,
        //            Position = i.Applicant.Appliedfor,
        //            InterviewerId = i.InterviewerId,
        //            InterviewerName = i.Interviewer != null
        //                ? i.Interviewer.FirstName + " " + i.Interviewer.LastName
        //                : "Not Assigned",
        //            Type = i.Mode,
        //            Status = i.Status,
        //            ScheduledDate = i.ScheduledDate,
        //            LocationUrl = i.LocationUrl,
        //            MeetingUrl = i.MeetingUrl,
        //            InterviewNote = i.InterviewNote
        //        }).ToListAsync();

        //    return Ok(interviews);
        //}

        //// 5. Get Today's Interviews Count
        //[HttpGet("today/count")]
        //public async Task<IActionResult> GetTodayInterviewsCount()
        //{
        //    var today = DateTime.UtcNow.Date;

        //    var count = await _context.Interviews
        //        .Where(i => i.ScheduledDate.HasValue && i.ScheduledDate.Value.Date == today)
        //        .CountAsync();

        //    return Ok(new
        //    {
        //        Message = $"Today's Interviews Count = {count}"
        //    });
        //}
    }
}
