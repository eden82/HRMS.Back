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

    public class AnnouncementController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnnouncementController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // 1. CREATE ANNOUNCEMENT 
        // =====================================================
        [HttpPost]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<ActionResult<object>> CreateAnnouncement([FromBody] AnnouncementDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var announcement = new Announcement
            {
                Title = dto.Title,
                Destination = dto.Destination,
                Categories = dto.Categories,
                Announcementcontent = dto.Announcementcontent,
                DepartmentID = dto.DepartmentID,
                TenantID = dto.TenantID,
                OrganizationID = dto.OrganizationID,
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = dto.ExpiryDate,
                IsActive = dto.IsActive
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            // ============ Extra Information ============
            var totalAnnouncements = await _context.Announcements.CountAsync();

            var now = DateTime.UtcNow;
            var currentAnnouncements = await _context.Announcements
                .Where(a => a.IsActive && (a.ExpiryDate == null || a.ExpiryDate >= now))
                .CountAsync();

            // Calculate reach percentage safely
            double reachPercent = totalAnnouncements > 0
                ? Math.Round(((double)currentAnnouncements / totalAnnouncements) * 100, 2)
                : 0;

            return Ok(new
            {
                Message = "Announcement created successfully!",
                Announcement = new
                {
                    announcement.Id,
                    announcement.Title,
                    announcement.Categories,
                    announcement.Destination,
                    announcement.Announcementcontent
                },
                Announcementemployee = new
                {
                    announcement.Id,
                    announcement.Title,
                    announcement.Categories,
                    announcement.CreatedAt,
                    announcement.Announcementcontent
                },
                TotalAnnouncements = totalAnnouncements,
                CurrentAnnouncements = currentAnnouncements,
                ReachPercentage = reachPercent
            });
        }


        // =====================================================
        // 2. GET ANNOUNCEMENT BY ID  (Fixes Missing Method)
        // =====================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<AnnouncementDTO>> GetAnnouncementById(Guid id)
        {
            var announcement = await _context.Announcements.FindAsync(id);

            if (announcement == null)
                return NotFound();

            var dto = new AnnouncementDTO
            {
                Id = announcement.Id,
                Title = announcement.Title,
                Destination = announcement.Destination,
                Categories = announcement.Categories,
                Announcementcontent = announcement.Announcementcontent,
                DepartmentID = announcement.DepartmentID,
                TenantID = announcement.TenantID,
                OrganizationID = announcement.OrganizationID,
                CreatedAt = announcement.CreatedAt,
                ExpiryDate = announcement.ExpiryDate,
                IsActive = announcement.IsActive
            };

            return Ok(dto);
        }

        // =====================================================
        // 3. UPDATE ANNOUNCEMENT
        // =====================================================
        [HttpPut("{id}")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<IActionResult> UpdateAnnouncement(Guid id, [FromBody] AnnouncementDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null) return NotFound();

            // Update fields safely
            announcement.Title = dto.Title ?? announcement.Title;
            announcement.Destination = dto.Destination ?? announcement.Destination;
            announcement.Categories = dto.Categories ?? announcement.Categories;
            announcement.Announcementcontent = dto.Announcementcontent ?? announcement.Announcementcontent;
            announcement.DepartmentID = dto.DepartmentID ?? announcement.DepartmentID;
            announcement.TenantID = dto.TenantID != Guid.Empty ? dto.TenantID : announcement.TenantID;
            announcement.OrganizationID = dto.OrganizationID != Guid.Empty ? dto.OrganizationID : announcement.OrganizationID;
            announcement.ExpiryDate = dto.ExpiryDate ?? announcement.ExpiryDate;
            announcement.IsActive = dto.IsActive;

            _context.Announcements.Update(announcement);
            await _context.SaveChangesAsync();


            // ============ Extra Information ============
            var totalAnnouncements = await _context.Announcements.CountAsync();

            var now = DateTime.UtcNow;
            var currentAnnouncements = await _context.Announcements
                .Where(a => a.IsActive && (a.ExpiryDate == null || a.ExpiryDate >= now))
                .CountAsync();

            // Calculate reach percentage safely
            double reachPercent = totalAnnouncements > 0
                ? Math.Round(((double)currentAnnouncements / totalAnnouncements) * 100, 2)
                : 0;

            return Ok(new
            {
                Message = "Announcement Updated successfully!",
                Announcement = new
                {
                    announcement.Id,
                    announcement.Title,
                    announcement.Categories,
                    announcement.Destination,
                    announcement.Announcementcontent
                },
                Announcementemployee = new
                {
                    announcement.Id,
                    announcement.Title,
                    announcement.Categories,
                    announcement.CreatedAt,
                    announcement.Announcementcontent
                },
                TotalAnnouncements = totalAnnouncements,
                CurrentAnnouncements = currentAnnouncements,
                ReachPercentage = reachPercent
            });
        }

        // =====================================================
        // 3. GET ALL ANNOUNCEMENTS 
        // =====================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnnouncementDTO>>> GetAllAnnouncements()
        {
            var announcements = await _context.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            var dtoList = announcements.Select(a => new AnnouncementDTO
            {
                Id = a.Id,
                Title = a.Title,
                Destination = a.Destination,
                Categories = a.Categories,
                Announcementcontent = a.Announcementcontent,
                DepartmentID = a.DepartmentID,
                TenantID = a.TenantID,
                OrganizationID = a.OrganizationID,
                CreatedAt = a.CreatedAt,
                ExpiryDate = a.ExpiryDate,
                IsActive = a.IsActive
            }).ToList();

            return Ok(dtoList);
        }

        // GET: api/Announcement/categories
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            var categories = await _context.Announcements
                .Select(a => a.Categories)
                .Distinct()
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/announcement/tenant/{tenantId}
        [HttpGet("tenant/{tenantId}")]
        public async Task<IActionResult> GetAnnouncementByTenant(Guid tenantId)
        {
            if (tenantId == Guid.Empty)
                return BadRequest("TenantId is required.");

            // Get all announcements for this tenant (OrganizationID = null)
            var announcements = await _context.Announcements
                .Where(a =>
                    (a.Destination == "All" ||
                     a.TenantID == tenantId) &&
                    a.OrganizationID == null)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

 
            //  Stats — only for this tenant (OrganizationID == null)
            var now = DateTime.UtcNow;

            var totalAnnouncements = await _context.Announcements
                .Where(a => a.TenantID == tenantId && a.OrganizationID == null)
                .CountAsync();

            var currentAnnouncements = await _context.Announcements
                .Where(a =>
                    a.TenantID == tenantId &&
                    a.OrganizationID == null &&
                    a.IsActive &&
                    (a.ExpiryDate == null || a.ExpiryDate >= now))
                .CountAsync();

            double reachPercentage = totalAnnouncements > 0
                ? Math.Round(((double)currentAnnouncements / totalAnnouncements) * 100, 2)
                : 0;

            // Structured response
            var response = new
            {
                announcementemployee = announcements.Select(a => new
                {
                    id = a.Id,
                    title = a.Title,
                    categories = a.Categories,
                    createdAt = a.CreatedAt,
                    announcementcontent = a.Announcementcontent
                }),
                totalAnnouncements,
                currentAnnouncements,
                reachPercentage
            };

            return Ok(response);
        }



        // GET: api/announcement/organization/{tenantId}/{organizationId}
        [HttpGet("organization/{tenantId}/{organizationId}")]
        public async Task<IActionResult> GetAnnouncementByTenant(Guid tenantId, Guid organizationId)
        {
            if (tenantId == Guid.Empty)
                return BadRequest("TenantId is required.");

            if (organizationId == Guid.Empty)
                return BadRequest("Organization is required.");

            // Get all announcements for this tenant + organization
            var announcements = await _context.Announcements
                .Where(a =>
                    (a.Destination == "All" ||
                     a.TenantID == tenantId) &&
                    a.OrganizationID == organizationId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            if (!announcements.Any())
                return NotFound(new { message = "No announcements found for this organization." });

            //  Stats — only for this organization under this tenant
            var now = DateTime.UtcNow;

            var totalAnnouncements = await _context.Announcements
                .Where(a => a.TenantID == tenantId && a.OrganizationID == organizationId)
                .CountAsync();

            var currentAnnouncements = await _context.Announcements
                .Where(a =>
                    a.TenantID == tenantId &&
                    a.OrganizationID == organizationId &&
                    a.IsActive &&
                    (a.ExpiryDate == null || a.ExpiryDate >= now))
                .CountAsync();

            double reachPercentage = totalAnnouncements > 0
                ? Math.Round(((double)currentAnnouncements / totalAnnouncements) * 100, 2)
                : 0;

            // Structured response
            var response = new
            {
                announcementemployee = announcements.Select(a => new
                {
                    id = a.Id,
                    title = a.Title,
                    categories = a.Categories,
                    createdAt = a.CreatedAt,
                    announcementcontent = a.Announcementcontent
                }),
                totalAnnouncements,
                currentAnnouncements,
                reachPercentage
            };

            return Ok(response);
        }


        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAnnouncementsForUser(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest("UserId is required.");
            // Fetch user details
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found.");
            var tenantId = user.TenantId;
            var organizationId = user.OrganizationId;
            // Fetch announcements for the user's tenant and organization
            var announcements = await _context.Announcements
                .Where(a =>
                    (a.Destination == "All" ||
                     a.TenantID == tenantId) &&
                    (a.OrganizationID == null || a.OrganizationID == organizationId))
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
 
            // Structured response
            var response = new
            {
                announcementemployee = announcements.Select(a => new
                {
                    id = a.Id,
                    title = a.Title,
                    categories = a.Categories,
                    createdAt = a.CreatedAt,
                    announcementcontent = a.Announcementcontent
                })
            };
            return Ok(response);
        }





        //// ===========================================
        //// GET: api/Announcement/search?category=HR
        //// ===========================================
        //[HttpGet("search")]
        //public async Task<ActionResult<IEnumerable<object>>> SearchAnnouncementsByCategory([FromQuery] string category)
        //{
        //    // Validate the input
        //    if (string.IsNullOrWhiteSpace(category))
        //        return BadRequest(new { message = "Category is required." });

        //    // Search announcements by category (case-insensitive)
        //    var announcements = await _context.Announcements
        //        .Where(a => a.Categories.ToLower().Contains(category.ToLower()))
        //        .OrderByDescending(a => a.CreatedAt)
        //        .Select(a => new
        //        {
        //            id = a.Id,
        //            title = a.Title,
        //            categories = a.Categories,
        //            destination = a.Destination,
        //            announcementcontent = a.Announcementcontent
        //        })
        //        .ToListAsync();

        //    if (!announcements.Any())
        //        return NotFound(new { message = $"No announcements found for category '{category}'." });

        //    return Ok(announcements);
        //}



        // =====================================================
        // 5. DELETE ANNOUNCEMENT 
        // =====================================================
        [HttpDelete("{id}")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<IActionResult> DeleteAnnouncement(Guid id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
                return NotFound(new { message = "Announcement not found" });

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Announcement deleted successfully" });
        }


        //Get(search) by title and Categories
        [HttpGet("search")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<IActionResult> SearchAnnouncements([FromQuery] string? title, [FromQuery] string? Categories)
        {
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(Categories))
                return BadRequest(new { message = "Provide at least a title or category to search." });

            var query = _context.Announcements.AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(a => a.Title.Contains(title));

            if (!string.IsNullOrWhiteSpace(Categories))
                query = query.Where(a => a.Categories.Contains(Categories));

            var results = await query
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Categories,
                    a.Announcementcontent,
                    a.DepartmentID,
                    a.CreatedAt
                })
                .ToListAsync();

            if (!results.Any())
                return NotFound(new { message = "No announcements found for the given criteria." });

            return Ok(results);
        }

    }
}
