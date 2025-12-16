using HRMS.Backend.Data;
using HRMS.Backend.Models;
using HRMS.Backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS.Backend.Filters;

namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
    public class ReviewQuestionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewQuestionController(AppDbContext context)
        {
            _context = context;
        }

        // ----------------------------------------------------
        // CREATE Review Question
        // ----------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> CreateReviewQuestion([FromBody] ReviewQuestionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // -------------------------------------------
            // Validation: Prevent duplicate question
            // -------------------------------------------
            var duplicate = await _context.ReviewQuestions
                .AnyAsync(q =>
                    q.QuestionText.ToLower() == dto.QuestionText.ToLower() &&
                    q.TenantId == dto.TenantId &&
                    q.OrganizationId == dto.OrganizationId);

            if (duplicate)
            {
                return BadRequest(new
                {
                    message = "This question already exists for this tenant/organization."
                });
            }

            // -------------------------------------------
            // Create Question
            // -------------------------------------------
            var question = new ReviewQuestion
            {
                Id = Guid.NewGuid(),
                QuestionText = dto.QuestionText,
                TenantId = dto.TenantId,
                OrganizationId = dto.OrganizationId,
                CreatedAt = DateTime.UtcNow
            };

            _context.ReviewQuestions.Add(question);
            await _context.SaveChangesAsync();

            // -------------------------------------------
            // Response
            // -------------------------------------------
            return Ok(new
            {
                Message = "Review question created successfully.",
                Data = new
                {
                    question.Id,
                    question.QuestionText,
                    question.TenantId,
                    Organization = question.OrganizationId?.ToString() ?? "Tenant-Level",
                    question.CreatedAt
                }
            });
        }

        //GET by TenantId
        [HttpGet("tenant/{tenantId}")]
        public async Task<IActionResult> GetTenantLevelQuestions(Guid tenantId)
        {
            // Fetch questions for tenant where OrganizationId is null
            var questions = await _context.ReviewQuestions
                .Where(q => q.TenantId == tenantId && q.OrganizationId == null)
                .Select(q => new
                {
                    q.Id,
                    q.QuestionText,
                    q.TenantId,
                    Organization = q.OrganizationId != null ? q.OrganizationId.ToString() : "Tenant-Level",
                    q.CreatedAt,
                    q.UpdatedAt
                })
                .ToListAsync();
 
            return Ok(questions);
        }
 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReviewQuestion(Guid id)
        {
            var question = await _context.ReviewQuestions
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound(new
                {
                    message = "Review question not found."
                });
            }

            _context.ReviewQuestions.Remove(question);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Review question deleted successfully.",
                deletedId = id
            });
        }


        // GET: api/reviewquestion/tenant/{tenantId}/simple
        [HttpGet("tenant/{tenantId}/simple")]
        public async Task<IActionResult> GetTenantQuestionsSimple(Guid tenantId)
        {
            var questions = await _context.ReviewQuestions
                .Where(q => q.TenantId == tenantId && q.OrganizationId == null)
                .Select(q => new
                {
                    q.Id,
                    q.QuestionText
                })
                .ToListAsync();

            return Ok(questions);
        }



    }
}
