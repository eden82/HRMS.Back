using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS.Backend.Data;
using HRMS.Backend.Models;
using HRMS.Backend.DTOs;
using HRMS.Backend.Filters;

namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoalController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GoalController(AppDbContext context)
        {
            _context = context;
        }

        // CRUD Operations for Goals

        // READ All Goals
        [HttpGet]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<ActionResult<IEnumerable<Goal>>> GetGoals()
        {
            var goals = await _context.Goals.ToListAsync();
            return Ok(goals);
        }

        // READ Goal by ID
        [HttpGet("{id}")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<ActionResult<Goal>> GetGoalById(Guid id)
        {
            var goal = await _context.Goals.FindAsync(id);
            if (goal == null)
                return NotFound(new { message = "Goal not found" });

            return Ok(goal);
        }

        // GET Distinct Categories
        [HttpGet("categories")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            var categories = await _context.Goals
                .Select(a => a.Category)
                .Distinct()
                .ToListAsync();

            return Ok(categories);
        }


        // CREATE Goal
        [HttpPost]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<IActionResult> CreateGoal([FromBody] GoalCreateDto dto)
        {
            // -------------------------------
            // 1. Validate Employee by Email
            // -------------------------------
            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Email.ToLower() == dto.EmployeeEmail.ToLower()
                                       && e.TenantId == dto.TenantID);

            if (employee == null)
            {
                ModelState.AddModelError(nameof(dto.EmployeeEmail), "Employee not found for this tenant.");
                return BadRequest(ModelState);
            }

            // -------------------------------
            // 2. Organization & Tenant Rules
            // -------------------------------

            // CASE Tenant Level Goal (OrganizationID = null)
            if (dto.OrganizationID == null)
            {
                if (employee.OrganizationId != null)
                {
                    ModelState.AddModelError(nameof(dto.OrganizationID),
                        "Employee must not belong to an organization for a tenant-level goal.");
                }

                if (employee.TenantId != dto.TenantID)
                {
                    ModelState.AddModelError(nameof(dto.TenantID),
                        "Employee tenant does not match goal tenant.");
                }
            }

            // CASE  Organization Level Goal
            else
            {
                if (employee.OrganizationId == null)
                {
                    ModelState.AddModelError(nameof(dto.OrganizationID),
                        "Employee must belong to an organization for an organization-level goal.");
                }
                else if (employee.OrganizationId != dto.OrganizationID)
                {
                    ModelState.AddModelError(nameof(dto.OrganizationID),
                        "Employee organization does not match goal organization.");
                }

                if (employee.TenantId != dto.TenantID)
                {
                    ModelState.AddModelError(nameof(dto.TenantID),
                        "Employee tenant does not match goal tenant.");
                }
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // -------------------------------
            // Save Goal
            // -------------------------------
            var goal = new Goal
            {
                Id = Guid.NewGuid(),
                EmployeeID = employee.EmployeeID,   // Employee found by email
                OrganizationID = dto.OrganizationID,
                TenantID = dto.TenantID,
                GoalTitle = dto.GoalTitle,
                Category = dto.Category,
                Priority = dto.Priority,
                Status = dto.Status,
                DueDate = dto.DueDate,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();

            // -------------------------------
            // Response (Match Job Structure)
            // -------------------------------
            var employeeName = employee.FirstName + " " + employee.LastName;

            var response = new
            {
                goal.Id,
                goal.GoalTitle,
                goal.Category,
                goal.Priority,
                goal.DueDate,
                goal.Status,
                Employee = employeeName,
                Organization = dto.OrganizationID?.ToString() ?? "Tenant-Level",
                goal.Description,
                goal.CreatedAt
            };

            // Active Goal Count
            var activeGoalsCount = await _context.Goals
                .Where(g => g.Status == "Active")
                .CountAsync();

            return Ok(new
            {
                Message = "Goal created successfully.",
                data = response,
                ActiveGoalsCount = activeGoalsCount
            });
        }



        // UPDATE goal
        [HttpPut("{id}")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<IActionResult> UpdateGoal(Guid id, [FromBody] GoalCreateDto dto)
        {
            // -------------------------------
            // Find Employee by Email
            // -------------------------------
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e =>
                    e.Email.ToLower() == dto.EmployeeEmail.ToLower() &&
                    e.TenantId == dto.TenantID
                );

            if (employee == null)
            {
                return BadRequest(new { message = "Employee not found for this tenant." });
            }

            // -------------------------------
            // Find Goal
            // -------------------------------
            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.Id == id);

            if (goal == null)
                return NotFound(new { message = "Goal not found." });

            // -------------------------------
            // Validate Tenant/Organization Rules
            // -------------------------------

            // Tenant-level goal
            if (dto.OrganizationID == null)
            {
                if (employee.OrganizationId != null)
                {
                    return BadRequest(new
                    {
                        message = "Employee must not belong to an organization for a tenant-level goal."
                    });
                }
            }
            else
            {
                // Org-level goal
                if (employee.OrganizationId == null)
                {
                    return BadRequest(new
                    {
                        message = "Employee must belong to an organization for an organization-level goal."
                    });
                }

                if (employee.OrganizationId != dto.OrganizationID)
                {
                    return BadRequest(new
                    {
                        message = "Employee organization does not match goal organization."
                    });
                }
            }

            if (employee.TenantId != dto.TenantID)
            {
                return BadRequest(new { message = "Employee tenant does not match goal tenant." });
            }

            // -------------------------------
            // Update Goal
            // -------------------------------
            goal.EmployeeID = employee.EmployeeID;
            goal.GoalTitle = dto.GoalTitle;
            goal.Category = dto.Category;
            goal.Priority = dto.Priority;
            goal.DueDate = dto.DueDate;
            goal.Status = dto.Status;
            goal.Description = dto.Description;
            goal.OrganizationID = dto.OrganizationID;
            goal.TenantID = dto.TenantID;
            goal.UpdatedAt = DateTime.UtcNow;

            _context.Goals.Update(goal);
            await _context.SaveChangesAsync();

            // -------------------------------
            // Response (same as POST)
            // -------------------------------
            var employeeName = employee.FirstName + " " + employee.LastName;

            var response = new
            {
                goal.Id,
                goal.GoalTitle,
                goal.Category,
                goal.Priority,
                goal.DueDate,
                goal.Status,
                Employee = employeeName,
                Organization = goal.OrganizationID?.ToString() ?? "Tenant-Level",
                goal.Description,
                goal.CreatedAt,
                goal.UpdatedAt
            };

            // -------------------------------
            // Active Goals Count
            // -------------------------------
            var activeGoalsCount = await _context.Goals
                .Where(g => g.Status != "Completed" &&
                       g.DueDate >= DateTime.UtcNow.Date)
                .CountAsync();

            return Ok(new
            {
                Message = "Goal updated successfully.",
                data = response,
                ActiveGoalsCount = activeGoalsCount
            });
        }




        // DELETE Goal
        [HttpDelete("{id}")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<IActionResult> DeleteGoal(Guid id)
        {
            var goal = await _context.Goals.FindAsync(id);
            if (goal == null)
                return NotFound(new { message = "Goal not found" });

            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Goal deleted successfully" });
        }



        // GET Goal Details by ID
        [HttpGet("details/{id}")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<IActionResult> GetGoalDetailsById(Guid id)
        {
            var goal = await _context.Goals
                .Where(g => g.Id == id)
                .Select(g => new
                {
                    EmployeeName = g.Employee != null
                        ? g.Employee.FirstName + " " + g.Employee.LastName
                        : "Unknown",

                    GoalTitle = g.GoalTitle,
                    Category = g.Category,
                    Priority = g.Priority,
                    DueDate = g.DueDate,
                    Status = g.Status,
                    Description = g.Description
                })
                .FirstOrDefaultAsync();

            if (goal == null)
                return NotFound(new { message = "Goal not found" });

            return Ok(goal);
        }

        // GET Active Goal Count
        [HttpGet("active/count")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<IActionResult> GetActiveGoalCount()
        {
            var activeGoalsCount = await _context.Goals
                .Where(g => g.Status == "Active")
                .CountAsync();

            return Ok(new { ActiveGoals = activeGoalsCount });
        }


        // GET: api/goals/search
        [HttpGet("search")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<IActionResult> SearchGoals([FromQuery] string? goalTitle, [FromQuery] string? category)
        {
            if (string.IsNullOrWhiteSpace(goalTitle) && string.IsNullOrWhiteSpace(category))
                return BadRequest("Provide either a goal title or a category.");

            var query = _context.Goals.AsQueryable();

            if (!string.IsNullOrWhiteSpace(goalTitle))
            {
                var titleLower = goalTitle.Trim().ToLower();
                query = query.Where(g => g.GoalTitle.ToLower().Contains(titleLower));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                var categoryLower = category.Trim().ToLower();
                query = query.Where(g => g.Category.ToLower().Contains(categoryLower));
            }

            var goals = await query
                .Include(g => g.Employee)
                .Select(g => new
                {
                    GoalId = g.Id,
                    GoalTitle = g.GoalTitle,
                    Category = g.Category,
                    Priority = g.Priority,
                    DueDate = g.DueDate,
                    Status = g.Status,
                    EmployeeName = g.Employee != null ? g.Employee.FirstName + " " + g.Employee.LastName : "Unknown",
                    Description = g.Description
                })
                .ToListAsync();

            if (!goals.Any())
                return NotFound("No goals found for the given criteria.");

            return Ok(goals);
        }

        // POST or PUT (update goal progress)
        [HttpPost("{id}/updateProgress")]
        public async Task<IActionResult> UpdateGoalProgress(Guid id, [FromBody] int newProgress)
        {
            var goal = await _context.Goals.FindAsync(id);
            if (goal == null) return NotFound();

            goal.GoalProcess = newProgress; // 0-100

            // Update status dynamically
            goal.Status = GetGoalStatus(goal.GoalProcess);

            await _context.SaveChangesAsync();

            // ---- Calculate Overall Progress After Update ----
            var totalGoals = await _context.Goals.CountAsync();
            var overallProgress = totalGoals > 0 ? await _context.Goals.AverageAsync(g => g.GoalProcess) : 0;

            //var completedGoals = await _context.Goals.CountAsync(g => g.GoalProcess >= 100);
            //var completionRate = totalGoals > 0 ? (completedGoals * 100.0) / totalGoals : 0;


            return Ok(new
            {
                UpdatedGoal = new
                {
                    goal.Id,
                    GoalProcess = goal.GoalProcess,
                    Status = goal.Status
                },
                Overall = new
                {
                    OverallProgress = overallProgress,
                    //CompletionRate = completionRate
                }
            });
        }

        // Helper method to compute status
        private string GetGoalStatus(int progress)
        {
            return progress >= 100 ? "Complete" : "InProgress";
        }


        // GET: api/goal/user/{userId}
        [HttpGet("user/{userId}")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR, Employee")]
        public async Task<IActionResult> GetGoalsByUserId(Guid userId)
        {
            // 1. Get the user
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.EmployeeId == null)
                return NotFound(new { message = "User or associated employee not found." });

            // 2. Get all goals for that employee
            var goals = await _context.Goals
                .Where(g => g.EmployeeID == user.EmployeeId)
                .Include(g => g.Organization)
                .Include(g => g.Tenant)
                .Select(g => new
                {
                    g.Id,
                    g.GoalTitle,
                    g.Category,
                    g.Priority,
                    g.Status,
                    g.DueDate,
                    g.Description,
                    g.GoalProcess,
                    OrganizationName = g.Organization != null ? g.Organization.Name : null,
                    TenantName = g.Tenant != null ? g.Tenant.Name : null
                })
                .ToListAsync();

            if (!goals.Any())
                return NotFound(new { message = "No goals found for this employee." });

            return Ok(goals);
        }

        // GET: api/goal/tenant/{tenantId}
        [HttpGet("tenant/{tenantId}")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR, Employee")]
        public async Task<IActionResult> GetTenantLevelGoals(Guid tenantId)
        {
            // Fetch goals for the tenant where OrganizationID is null
            var goals = await _context.Goals
                .Where(g => g.TenantID == tenantId && g.OrganizationID == null)
                .Include(g => g.Employee)
                .ToListAsync();

 
            // ----------- ACTIVE GOALS ------------
            var today = DateTime.UtcNow.Date;
            var activeGoalsCount = goals
                .Count(g => g.Status != "Complete" && g.DueDate.Date >= today);

            // ----------- REVIEWS DUE ------------
            var reviewsDueCount = await _context.RequestFeedbacks
                .CountAsync(r => r.FeedbackDeadline.Date <= today &&
                                 !r.FeedbackResponses.Any());

            // ----------- GOAL COMPLETION (Percentage) ------------
            int totalGoals = goals.Count;
            int completedGoals = goals.Count(g => g.Status == "Complete");

            double completionPercentage = totalGoals == 0
                ? 0
                : Math.Round((completedGoals / (double)totalGoals) * 100, 2);

            // ----------- GOALS LIST RESPONSE ------------
            var goalList = goals.Select(g => new
            {
                g.Id,
                g.GoalTitle,
                g.Category,
                g.Priority,
                g.Status,
                g.DueDate,
                g.Description,
                g.GoalProcess,
                EmployeeName = g.Employee != null ? g.Employee.FirstName + " " + g.Employee.LastName : "Unknown"
            });

            return Ok(new
            {
                TotalGoals = totalGoals,
                ActiveGoals = activeGoalsCount,
                ReviewsDue = reviewsDueCount,
                CompletionPercentage = completionPercentage,
                Goals = goalList
            });
        }



    }
}
