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
    [RoleAuthorize("SuperAdmin , SystemAdmin , HR, Employee")]
    public class PerformanceReviewController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PerformanceReviewController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [RoleAuthorize("SuperAdmin, SystemAdmin, HR")]
        public async Task<IActionResult> CreatePerformanceReview([FromBody] PerformanceReviewCreateDto dto)
        {
            // 1. Validate Employee
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e =>
                    e.Email.ToLower() == dto.EmployeeEmail.ToLower() &&
                    e.TenantId == dto.TenantId);

            if (employee == null)
                return BadRequest("Employee not found for this tenant.");

            if (dto.Questions == null || !dto.Questions.Any())
                return BadRequest("At least one review question is required.");

            // 2. Fetch Questions
            var questionIds = dto.Questions.Select(q => q.ReviewQuestionId).ToList();

            var questions = await _context.ReviewQuestions
                .Where(q => questionIds.Contains(q.Id))
                .ToListAsync();

            if (questions.Count != questionIds.Count)
                return BadRequest("One or more review questions are invalid.");

            // 3. Tenant / Organization validation
            foreach (var q in questions)
            {
                if (q.TenantId != dto.TenantId)
                    return BadRequest("Question tenant mismatch.");

                if (dto.OrganizationId == null && q.OrganizationId != null)
                    return BadRequest("Organization question cannot be used at tenant level.");

                if (dto.OrganizationId != null && q.OrganizationId != dto.OrganizationId)
                    return BadRequest("Organization mismatch in review questions.");
            }

            // 4. Create MAIN review
            var review = new PerformanceReview
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.EmployeeID,
                TenantID = dto.TenantId,
                OrganizationID = dto.OrganizationId,
                ReviewType = dto.ReviewType,
                OverallFeedback = dto.OverallFeedback,
                ReviewCycle = dto.ReviewCycle,
                CreatedAt = DateTime.UtcNow
            };

            _context.PerformanceReviews.Add(review);

            // 5. Create DETAIL records
            var details = dto.Questions.Select(q => new PerformanceReviewDetail
            {
                Id = Guid.NewGuid(),
                PerformanceReviewId = review.Id,
                ReviewQuestionId = q.ReviewQuestionId,
                Rating = q.Rating,
                FeedBack = q.FeedBack,
                TenantID = dto.TenantId,
                OrganizationID = dto.OrganizationId,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            _context.PerformanceReviewDetails.AddRange(details);

            await _context.SaveChangesAsync();

            // 6. Response
            return Ok(new
            {
                Message = "Performance review created successfully.",
                ReviewId = review.Id,
                QuestionsCount = details.Count
            });
        }



        [HttpPut("{id}")]
        [RoleAuthorize("SuperAdmin, SystemAdmin, HR")]
        public async Task<IActionResult> UpdatePerformanceReview(Guid id, [FromBody] PerformanceReviewCreateDto dto)
        {
            var review = await _context.PerformanceReviews
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
                return NotFound(new { message = "Performance review not found." });

            // Validate employee
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e =>
                    e.Email.ToLower() == dto.EmployeeEmail.ToLower() &&
                    e.TenantId == dto.TenantId);

            if (employee == null)
                return BadRequest("Employee not found for this tenant.");

            if (dto.Questions == null || !dto.Questions.Any())
                return BadRequest("At least one review question is required.");

            // Validate questions
            var questionIds = dto.Questions.Select(q => q.ReviewQuestionId).ToList();

            var questions = await _context.ReviewQuestions
                .Where(q => questionIds.Contains(q.Id))
                .ToListAsync();

            if (questions.Count != questionIds.Count)
                return BadRequest("One or more review questions are invalid.");

            foreach (var q in questions)
            {
                if (q.TenantId != dto.TenantId)
                    return BadRequest("Question tenant mismatch.");

                if (dto.OrganizationId == null && q.OrganizationId != null)
                    return BadRequest("Organization question cannot be used at tenant level.");

                if (dto.OrganizationId != null && q.OrganizationId != dto.OrganizationId)
                    return BadRequest("Organization mismatch in review questions.");
            }

            // Update MAIN review
            review.EmployeeId = employee.EmployeeID;
            review.TenantID = dto.TenantId;
            review.OrganizationID = dto.OrganizationId;
            review.ReviewType = dto.ReviewType;
            review.OverallFeedback = dto.OverallFeedback;
            review.ReviewCycle = dto.ReviewCycle;
            review.UpdatedAt = DateTime.UtcNow;

            // Remove old question details
            var oldDetails = await _context.PerformanceReviewDetails
                .Where(d => d.PerformanceReviewId == review.Id)
                .ToListAsync();

            _context.PerformanceReviewDetails.RemoveRange(oldDetails);

            // Insert new details
            var newDetails = dto.Questions.Select(q => new PerformanceReviewDetail
            {
                Id = Guid.NewGuid(),
                PerformanceReviewId = review.Id,
                ReviewQuestionId = q.ReviewQuestionId,
                Rating = q.Rating,
                FeedBack = q.FeedBack,
                TenantID = dto.TenantId,
                OrganizationID = dto.OrganizationId,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            _context.PerformanceReviewDetails.AddRange(newDetails);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Performance review updated successfully.",
                ReviewId = review.Id,
                QuestionsCount = newDetails.Count
            });
        }



        [HttpGet("{id}")]
        [RoleAuthorize("SuperAdmin, SystemAdmin, HR, Employee")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var review = await _context.PerformanceReviews
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
                return NotFound(new { message = "Performance review not found." });

            var questions = await _context.PerformanceReviewDetails
                .Where(d => d.PerformanceReviewId == id)
                .Include(d => d.ReviewQuestion)
                .Select(d => new
                {
                    d.ReviewQuestionId,
                    Question = d.ReviewQuestion.QuestionText,
                    d.Rating,
                    d.FeedBack
                })
                .ToListAsync();

            return Ok(new
            {
                review.Id,
                Employee = review.Employee.FirstName + " " + review.Employee.LastName,
                review.TenantID,
                review.OrganizationID,
                review.ReviewType,
                review.OverallFeedback,
                review.ReviewCycle,
                review.CreatedAt,
                review.UpdatedAt,
                Questions = questions
            });
        }


        // GET Performance Reviews by TenantId (tenant-level)
        [HttpGet("tenant/{tenantId}")]
        [RoleAuthorize("SuperAdmin, SystemAdmin, HR, Employee")]
        public async Task<IActionResult> GetTenantLevelReviews(Guid tenantId)
        {
            // 1. Fetch tenant-level reviews
            var reviews = await _context.PerformanceReviews
                .Where(r => r.TenantID == tenantId && r.OrganizationID == null)
                .Include(r => r.Employee)
                .ToListAsync();

            //if (!reviews.Any())
            //    return NotFound(new { message = "No tenant-level performance reviews found for this tenant." });

            // 2. Fetch tenant-level goals
            var goals = await _context.Goals
                .Where(g => g.TenantID == tenantId && g.OrganizationID == null)
                .ToListAsync();

            // ----------- ACTIVE GOALS ------------
            var today = DateTime.UtcNow.Date;
            var activeGoalsCount = goals
                .Count(g => g.Status != "Complete" && g.DueDate.Date >= today);

            // ----------- REVIEWS DUE ------------
            var reviewsDueCount = await _context.RequestFeedbacks
                .CountAsync(r =>
                    r.FeedbackDeadline.Date <= today &&
                    !r.FeedbackResponses.Any());

            // ----------- GOAL COMPLETION (Percentage) ------------
            int totalGoals = goals.Count;
            int completedGoals = goals.Count(g => g.Status == "Complete");

            double completionPercentage = totalGoals == 0
                ? 0
                : Math.Round((completedGoals / (double)totalGoals) * 100, 2);

            // 3. Prepare review data (NEW)
            var reviewData = reviews.Select(r => new
            {
                r.Id,
                Employee = r.Employee != null
                    ? r.Employee.FirstName + " " + r.Employee.LastName
                    : "Unknown",
                r.TenantID,
                r.OrganizationID,
                r.ReviewType,
                r.OverallFeedback,
                r.ReviewCycle,
                r.CreatedAt,
                r.UpdatedAt,

                //  Rating summary from details
                AverageRating = _context.PerformanceReviewDetails
                    .Where(d => d.PerformanceReviewId == r.Id)
                    .Average(d => (double?)d.Rating) ?? 0,

                QuestionsCount = _context.PerformanceReviewDetails
                    .Count(d => d.PerformanceReviewId == r.Id)
            });

            // 4. Return SAME STRUCTURE you asked for
            return Ok(new
            {
                Message = "Tenant-level performance reviews retrieved successfully.",
                Data = reviewData,
                Metrics = new
                {
                    activeGoalsCount = activeGoalsCount,
                    reviewsDueCount = reviewsDueCount,
                    completedGoals = completedGoals,
                    completionPercentage = completionPercentage
                }
            });
        }





        // GET: api/performancereview/user/{userId}
        [HttpGet("user/{userId}")]
        [RoleAuthorize("SuperAdmin, SystemAdmin, HR, Employee")]
        public async Task<IActionResult> GetPerformanceReviewsByUser(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest("UserId is required.");

            //  Find the user
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found.");

            //  Get EmployeeId from User
            if (user.EmployeeId == null)
                return BadRequest("User is not linked to an employee.");

            var employeeId = user.EmployeeId.Value;

            //  Get all performance reviews for this employee
            var reviews = await _context.PerformanceReviews
                .Where(r => r.EmployeeId == employeeId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            //if (!reviews.Any())
            //    return NotFound(new { message = "No performance reviews found for this employee." });

            //  Get all review details (questions, ratings, feedback)
            var reviewIds = reviews.Select(r => r.Id).ToList();

            var details = await _context.PerformanceReviewDetails
                .Where(d => reviewIds.Contains(d.PerformanceReviewId))
                .Include(d => d.ReviewQuestion)
                .ToListAsync();

            //  Structured response
            var response = new
            {
                performancereviewemployee = reviews.Select(r => new
                {
                    reviewId = r.Id,
                    r.ReviewType,
                    r.ReviewCycle,
                    r.OverallFeedback,
                    r.CreatedAt,

                    questions = details
                        .Where(d => d.PerformanceReviewId == r.Id)
                        .Select(d => new
                        {
                            d.ReviewQuestionId,
                            question = d.ReviewQuestion.QuestionText,
                            d.Rating,
                            feedback = d.FeedBack
                        })
                })
            };

            return Ok(response);
        }










        [HttpDelete("{id}")]
        [RoleAuthorize("SuperAdmin, SystemAdmin, HR")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var review = await _context.PerformanceReviews
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
                return NotFound(new { message = "Performance review not found." });

            var details = await _context.PerformanceReviewDetails
                .Where(d => d.PerformanceReviewId == id)
                .ToListAsync();

            _context.PerformanceReviewDetails.RemoveRange(details);
            _context.PerformanceReviews.Remove(review);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Performance review deleted successfully." });
        }

    }
}
