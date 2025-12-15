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
    public class JobController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JobController(AppDbContext context)
        {
            _context = context;
        }

        // CREATE Job
        [HttpPost]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<IActionResult> CreateJob([FromBody] JobCreateDto dto)
        {



            // Get Department by name
            Guid? departmentId = null;
            if (!string.IsNullOrWhiteSpace(dto.DepartmentName))
            {
                var department = await _context.Departments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.DepartmentName.ToLower() == dto.DepartmentName.ToLower()
                                           && d.TenantId == dto.TenantID);

                if (department == null)
                {
                    ModelState.AddModelError(nameof(dto.DepartmentName), "Department not found for this tenant.");
                }
                else
                {
                    // Check organization match if OrganizationId is provided
                    if (dto.OrganizationId.HasValue && dto.OrganizationId.Value != Guid.Empty)
                    {
                        if (department.OrganizationId != dto.OrganizationId)
                        {
                            ModelState.AddModelError(nameof(dto.DepartmentName),
                                "Department must belong to the same organization as the job.");
                        }
                    }
                    else
                    {
                        //  Tenant-level department (no organization)
                        if (department.OrganizationId != null)
                        {
                            ModelState.AddModelError(nameof(dto.DepartmentName),
                                "Department must not belong to an organization when job is tenant-level.");
                        }
                    }

                    departmentId = department.Id;
                }
            }


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var job = new Job
            {
                Id = Guid.NewGuid(), // Generate new GUID
                JobTitle = dto.JobTitle,
                DepartmentID = departmentId,
                OrganizationId = dto.OrganizationId,
                TenantID = dto.TenantID,
                Location = dto.Location,
                JobType = dto.JobType,
                SalaryRange = dto.SalaryRange,
                ApplicationDeadline = dto.ApplicationDeadline,
                JobDescription = dto.JobDescription,
                Requirement = dto.Requirement,
                CreatedAt = DateTime.UtcNow
            };


            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            // Fetch organization or tenant name for display
            string? organizationOrTenantName = null;

            if (job.OrganizationId.HasValue)
            {
                var org = await _context.Organizations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.Id == job.OrganizationId.Value);
                organizationOrTenantName = org?.Name;
            }
            else
            {
                var tenant = await _context.Tenants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == job.TenantID);
                organizationOrTenantName = tenant?.Name;
            }

            // Build the response with all details
            var response = new
            {
                job.Id,
                job.JobTitle,
                job.JobType,
                job.SalaryRange,
                job.Location,
                job.ApplicationDeadline,
                job.JobDescription,
                job.Requirement,
                OrganizationOrTenant = organizationOrTenantName ?? "N/A",
                DepartmentName = dto.DepartmentName ?? "N/A",
                job.CreatedAt
            };

            //  Get updated active jobs count after posting
            var today = DateTime.UtcNow.Date;
            var activeJobsCount = await _context.Jobs
                .Where(j => j.ApplicationDeadline.HasValue && j.ApplicationDeadline >= today)
                .CountAsync();

            // Return the same JSON as GetActiveJobs
            var jobJson = new
            {
                JobTitle = job.JobTitle,
                Department = job.DepartmentID,
                Location = job.Location,
                ApplicationDeadline = job.ApplicationDeadline


            };

            return Ok(new
            {
                message = "Job created successfully.",
                jobJson = jobJson,
                data = response,
                ActiveJobsCount = activeJobsCount
            });
        }

        // READ All Jobs
        [HttpGet]
        public async Task<IActionResult> GetJobs()
        {
            var jobs = await _context.Jobs.ToListAsync();
            return Ok(jobs);
        }

        // READ Job by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobById(Guid id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound();
            return Ok(job);
        }

        // UPDATE Job
        [HttpPut("{id}")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<IActionResult> UpdateJob(Guid id, [FromBody] JobCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return NotFound(new { message = $"Job with Id {id} not found." });

            // ---------- VALIDATE DEPARTMENT BY NAME ----------
            Guid? departmentId = null;
            if (!string.IsNullOrWhiteSpace(dto.DepartmentName))
            {
                var department = await _context.Departments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.DepartmentName.ToLower() == dto.DepartmentName.ToLower()
                                           && d.TenantId == dto.TenantID);

                if (department == null)
                    return BadRequest(new { message = "Department not found for this tenant." });

                // Optional: check organization match
                if (dto.OrganizationId.HasValue && dto.OrganizationId != Guid.Empty)
                {
                    if (department.OrganizationId != dto.OrganizationId)
                        return BadRequest(new { message = "Department must belong to the same organization as the job." });
                }
                else
                {
                    // Tenant-level department
                    if (department.OrganizationId != null)
                        return BadRequest(new { message = "Department must not belong to an organization for tenant-level job." });
                }

                departmentId = department.Id;
            }

            // ---------- UPDATE JOB ----------
            job.JobTitle = dto.JobTitle;
            job.DepartmentID = departmentId;
            job.OrganizationId = dto.OrganizationId;
            job.TenantID = dto.TenantID;
            job.Location = dto.Location;
            job.JobType = dto.JobType;
            job.SalaryRange = dto.SalaryRange;
            job.ApplicationDeadline = dto.ApplicationDeadline;
            job.JobDescription = dto.JobDescription;
            job.Requirement = dto.Requirement;
            job.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // ---------- BUILD RESPONSE ----------
            string? organizationOrTenantName = null;
            if (job.OrganizationId.HasValue)
            {
                var org = await _context.Organizations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.Id == job.OrganizationId.Value);
                organizationOrTenantName = org?.Name;
            }
            else
            {
                var tenant = await _context.Tenants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == job.TenantID);
                organizationOrTenantName = tenant?.Name;
            }

            var response = new
            {
                job.Id,
                job.JobTitle,
                job.JobType,
                job.SalaryRange,
                job.Location,
                job.ApplicationDeadline,
                job.JobDescription,
                job.Requirement,
                OrganizationOrTenant = organizationOrTenantName ?? "N/A",
                DepartmentName = dto.DepartmentName ?? "N/A",
                job.UpdatedAt
            };

            // Optional: count active jobs
            var today = DateTime.UtcNow.Date;
            var activeJobsCount = await _context.Jobs
                .Where(j => j.ApplicationDeadline.HasValue && j.ApplicationDeadline >= today)
                .CountAsync();

            return Ok(new
            {
                message = "Job updated successfully.",
                data = response,
                ActiveJobsCount = activeJobsCount
            });
        }


        // DELETE Job
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(Guid id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound();

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Job deleted successfully" });
        }

        // GET: api/job/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveJobs()
        {
            var today = DateTime.UtcNow.Date;

            var activeJobs = await _context.Jobs
                .Where(j => j.ApplicationDeadline.HasValue && j.ApplicationDeadline >= today)
                .OrderBy(j => j.ApplicationDeadline)
                .Select(j => new
                {
                    JobTitle = j.JobTitle,
                    Department = j.DepartmentID,
                    Location = j.Location,
                    ApplicationDeadline = j.ApplicationDeadline
                })
                .ToListAsync();

            if (activeJobs.Count == 0)
                return NotFound(new { message = "No active jobs available." });

            return Ok(activeJobs);
        }

        // GET: api/job/active/count
        [HttpGet("active/count")]
        public async Task<IActionResult> GetActiveJobsCount()
        {
            var today = DateTime.UtcNow.Date;

            var activeJobsCount = await _context.Jobs
                .CountAsync(j => j.ApplicationDeadline.HasValue && j.ApplicationDeadline >= today);

            return Ok(new { activeJobs = activeJobsCount });
        }



        // GET: api/job/search
        [HttpGet("search")]
        public async Task<IActionResult> SearchJobs(
            [FromQuery] string? jobType,
            [FromQuery] string? departmentName,
            [FromQuery] double? hoursAgo // e.g. 1, 15, 72
        )
        {
            var query = _context.Jobs
                .Include(j => j.Department)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(jobType) ||
                !string.IsNullOrWhiteSpace(departmentName) ||
                (hoursAgo.HasValue && hoursAgo > 0))
            {
                var since = DateTime.UtcNow.AddHours(-(hoursAgo ?? 0));

                query = query.Where(j =>
                    (!string.IsNullOrWhiteSpace(jobType) && j.JobType.Contains(jobType)) ||
                    (!string.IsNullOrWhiteSpace(departmentName) && j.Department != null && j.Department.DepartmentName.Contains(departmentName)) ||
                    (hoursAgo.HasValue && j.CreatedAt >= since)
                );
            }


            // Execute and calculate how many hours ago each job was created
            var jobs = await query
                .Select(j => new
                {
                    j.JobTitle,
                    Department = j.Department != null ? j.Department.DepartmentName : null,
                    j.JobType,
                    j.Location,
                    j.CreatedAt,
                    HoursAgo = Math.Round((DateTime.UtcNow - j.CreatedAt).TotalHours, 1),
                    j.ApplicationDeadline,
                    j.SalaryRange,
                    j.JobDescription,
                    j.Requirement,
                    organizationOrTenant = j.Organization != null
                        ? j.Organization.Name
                        : (j.Tenant != null ? j.Tenant.Name : "N/A"),
                    departmentName = j.Department != null
                        ? j.Department.DepartmentName
                        : "N/A"
                })
                .ToListAsync();

            if (jobs.Count == 0)
                return NotFound(new { message = "No jobs match the search criteria." });

            return Ok(jobs);
        }




        // READ All Jobs
        [HttpGet("data")]
        public async Task<IActionResult> GetJob()
        {
            var today = DateTime.UtcNow.Date;

            var jobs = await _context.Jobs
                .Include(j => j.Department)
                .Include(j => j.Organization)
                .Include(j => j.Tenant)
                .AsNoTracking()
                .Select(j => new
                {
                    id = j.Id,
                    jobTitle = j.JobTitle,
                    jobType = j.JobType,
                    salaryRange = j.SalaryRange,
                    location = j.Location,
                    applicationDeadline = j.ApplicationDeadline,
                    jobDescription = j.JobDescription,
                    requirement = j.Requirement,
                    organizationOrTenant = j.Organization != null
                        ? j.Organization.Name
                        : (j.Tenant != null ? j.Tenant.Name : "N/A"),
                    departmentName = j.Department != null
                        ? j.Department.DepartmentName
                        : "N/A",
                    createdAt = j.CreatedAt
                })
                .ToListAsync();

            // Count active jobs
            var activeJobsCount = await _context.Jobs
                .CountAsync(j => j.ApplicationDeadline.HasValue && j.ApplicationDeadline >= today);

            if (jobs.Count == 0)
                return NotFound(new { message = "No jobs available." });

            // Return full structured response
            return Ok(new
            {
                message = "Jobs retrieved successfully.",
                data = jobs,
                activeJobsCount = activeJobsCount
            });
        }


        // GET: api/job/dashboard/org/{tenantId}/{organizationId}
        // Active jobs for a specific organization within a tenant
        [HttpGet("dashboard/org/{tenantId}/{organizationId}")]
        public async Task<IActionResult> GetJobsByOrganization(Guid tenantId, Guid organizationId)
        {
            var today = DateTime.UtcNow.Date;

            // Get active jobs for tenant + organization
            var jobs = await _context.Jobs
                .Include(j => j.Organization)
                .Include(j => j.Tenant)
                .AsNoTracking()
                .Where(j => j.TenantID == tenantId
                            && j.OrganizationId == organizationId
                            && j.ApplicationDeadline.HasValue
                            && j.ApplicationDeadline >= today)
                .Select(j => new
                {
                    j.Id,
                    j.JobTitle,
                    j.JobType,
                    j.SalaryRange,
                    j.Location,
                    j.ApplicationDeadline,
                    j.JobDescription,
                    j.Requirement,
                    organizationOrTenant = j.Organization != null ? j.Organization.Name : "N/A",
                    departmentName = j.Department != null ? j.Department.DepartmentName : "N/A",
                    j.CreatedAt
                })
                .ToListAsync();

            var activeJobsCount = jobs.Count;
            var jobIds = jobs.Select(j => j.Id).ToList();
            var totalApplications = await _context.ApplicantJobs
                .CountAsync(a => jobIds.Contains(a.JobId));
            var interviewsToday = await _context.Interviews
            .CountAsync(i => i.ScheduledOn.HasValue && i.ScheduledOn.Value.Date == today);


            return Ok(new
            {
                message = "Organization-level dashboard retrieved successfully.",
                activeJobsCount,
                totalApplications,
                interviewsToday,
                jobs
            });
        }

        // GET: api/job/dashboard/tenant/{tenantId}
        // Active jobs for tenant only (organizationId is null)
        [HttpGet("dashboard/tenant/{tenantId}")]
        public async Task<IActionResult> GetJobsByTenantOnly(Guid tenantId)
        {
            var today = DateTime.UtcNow.Date;

            var jobs = await _context.Jobs
                .Include(j => j.Department)
                .Include(j => j.Tenant)
                .AsNoTracking()
                .Where(j => j.TenantID == tenantId
                            && j.OrganizationId == null
                            && j.ApplicationDeadline.HasValue
                            && j.ApplicationDeadline >= today)
                .Select(j => new
                {
                    j.Id,
                    j.JobTitle,
                    j.JobType,
                    j.SalaryRange,
                    j.Location,
                    j.ApplicationDeadline,
                    j.JobDescription,
                    j.Requirement,
                    organizationOrTenant = j.Tenant != null ? j.Tenant.Name : "N/A",
                    departmentName = j.Department != null ? j.Department.DepartmentName : "N/A",
                    j.CreatedAt
                })
                .ToListAsync();

            var activeJobsCount = jobs.Count;
            var jobIds = jobs.Select(j => j.Id).ToList();
            var totalApplications = await _context.ApplicantJobs
                .CountAsync(a => jobIds.Contains(a.JobId));
            var interviewsToday = await _context.Interviews
            .CountAsync(i => i.ScheduledOn.HasValue && i.ScheduledOn.Value.Date == today);


            return Ok(new
            {
                message = "Tenant-level dashboard retrieved successfully.",
                activeJobsCount,
                totalApplications,
                interviewsToday,
                jobs
            });
        }



    }
}
