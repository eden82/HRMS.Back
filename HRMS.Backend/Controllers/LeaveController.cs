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
    public class LeaveController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LeaveController(AppDbContext context)
        {
            _context = context;
        }

        ////  GET TOTAL LEAVE REQUESTS
        //[HttpGet("total-requests")]
        //public async Task<IActionResult> GetTotalRequests()
        //{
        //    var totalRequests = await _context.Leaves.CountAsync();
        //    return Ok(new { TotalRequests = totalRequests });
        //}

        ////  GET TOTAL PENDING REQUESTS
        //[HttpGet("pending-requests")]
        //public async Task<IActionResult> GetPendingRequests()
        //{
        //    var pendingRequests = await _context.Leaves
        //        .CountAsync(l => l.Status == "Pending");

        //    return Ok(new { PendingRequests = pendingRequests });
        //}

        ////  GET TOTAL APPROVED REQUESTS
        //[HttpGet("approved-requests")]
        //public async Task<IActionResult> GetApprovedRequests()
        //{
        //    var approvedRequests = await _context.Leaves
        //        .CountAsync(l => l.Status == "Approved");

        //    return Ok(new { ApprovedRequests = approvedRequests });
        //}

        ////  GET TOTAL REJECTED REQUESTS
        //[HttpGet("rejected-requests")]
        //public async Task<IActionResult> GetRejectedRequests()
        //{
        //    var rejectedRequests = await _context.Leaves
        //        .CountAsync(l => l.Status == "Rejected");

        //    return Ok(new { RejectedRequests = rejectedRequests });
        //}

        //  GET ALL LEAVE REQUESTS
        [HttpGet("requests")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetAllLeaveRequests()
        {
            var leaves = await _context.Leaves
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Select(l => new LeaveRequestDto
                {
                    LeaveID = l.Id,  // updated
                    EmployeeName = l.Employee != null ? l.Employee.FirstName + " " + l.Employee.LastName : "Unknown",
                    LeaveType = l.LeaveType != null ? l.LeaveType.Name : "N/A",
                    Duration = l.StartDate.ToString("yyyy-MM-dd") + " → " + l.EndDate.ToString("yyyy-MM-dd"),
                    Reason = l.Reason ?? "N/A",
                    Status = l.Status ?? "Pending"
                })
                .ToListAsync();

            return Ok(leaves);
        }

        //  GET ALL UNIQUE STATUSES
        [HttpGet("statuses")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<IActionResult> GetAllStatuses()
        {
            var statuses = await _context.Leaves
                .Select(l => l.Status)
                .Distinct()
                .ToListAsync();

            return Ok(statuses);
        }



        //  UPDATE LEAVE STATUS
        [HttpPut("{id}/status")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<IActionResult> UpdateLeaveStatus(Guid id, [FromBody] UpdateLeaveStatusDto dto)
        {
            try
            {
                var leave = await _context.Leaves
                    .Include(l => l.Employee)
                    .Include(l => l.LeaveType)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (leave == null)
                    return NotFound(new { Message = "Leave request not found" });

                if (dto.Status != "Approved" && dto.Status != "Rejected")
                    return BadRequest(new { Message = "Invalid status. Only 'Approved' or 'Rejected' allowed." });

                //  Prevent re-approval/rejection
                if (leave.Status == "Approved" || leave.Status == "Rejected")
                    return BadRequest(new { Message = $"Leave request is already {leave.Status} and cannot be updated again." });

                leave.Status = dto.Status;
                leave.ManagerComment = dto.ManagerComment;
                leave.UpdatedAt = DateTime.UtcNow;

                // Define year range for remainingDays calculation
                var yearStart = new DateTime(DateTime.UtcNow.Year, 1, 1);
                var yearEnd = new DateTime(DateTime.UtcNow.Year, 12, 31);

                // Calculate used leave days for this employee and leave type
                var usedDaysBefore = await _context.Leaves
                    .Where(l =>
                        l.EmployeeId == leave.EmployeeId &&
                        l.LeaveTypeId == leave.LeaveTypeId &&
                        l.StartDate >= yearStart &&
                        l.EndDate <= yearEnd &&
                        l.Status == "Approved")
                    .SumAsync(l => EF.Functions.DateDiffDay(l.StartDate, l.EndDate) + 1);

                var requestedDays = (leave.EndDate - leave.StartDate).Days + 1;
                var maxDays = leave.LeaveType?.MaxDays ?? 0;

                int remainingDaysAfter = maxDays - usedDaysBefore;

                // If Approving → Deduct leave days
                if (dto.Status == "Approved")
                {
                    if (remainingDaysAfter < requestedDays)
                    {
                        return BadRequest(new
                        {
                            Message = "Insufficient remaining leave days.",
                            RemainingBefore = remainingDaysAfter,
                            RequestedDays = requestedDays,
                            MaxDays = maxDays
                        });
                    }

                    // Subtract the approved days
                    remainingDaysAfter -= requestedDays;
                }

                _context.Leaves.Update(leave);
                await _context.SaveChangesAsync();

                var duration = requestedDays;

                return Ok(new
                {
                    Message = $"Leave request has been {dto.Status}",
                    Leave = new
                    {
                        LeaveId = leave.Id,
                        EmployeeName = leave.Employee != null
                            ? leave.Employee.FirstName + " " + leave.Employee.LastName
                            : "Unknown",
                        LeaveType = leave.LeaveType?.Name ?? "N/A",
                        duration = $"{Math.Round((leave.EndDate - leave.StartDate).TotalHours, 2)}hr",
                        Days = duration,
                        Reason = leave.Reason ?? "N/A",
                        Status = leave.Status,
                        RemainingDays = remainingDaysAfter
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating leave status", Error = ex.Message });
            }
        }





        //  SUBMIT LEAVE REQUEST
        [HttpPost]
        public async Task<IActionResult> SubmitLeaveRequest([FromBody] EmployeeLeaveRequestDto dto)
        {
            if (dto == null)
                return BadRequest("Body required.");

            if (dto.UserId == Guid.Empty)
                return BadRequest("UserId is required.");

            // Find the user
            var user = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == dto.UserId);

            if (user == null)
                return BadRequest("User not found.");

            if (user.EmployeeId == null)
                return BadRequest("User is not linked to an employee.");

            var employeeId = user.EmployeeId.Value;

            var employee = await _context.Employees.AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeID == employeeId);

            if (employee == null)
                return NotFound(new { Message = "Employee not found." });

            var leaveType = await _context.LeaveTypes.AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == dto.LeaveTypeId);

            if (leaveType == null)
                return BadRequest("Invalid leave type.");

            // Tenant/org validation
            if (dto.OrganizationId == null)
            {
                if (leaveType.TenantId != dto.TenantId ||
                    employee.TenantId != dto.TenantId)
                    return BadRequest(new { Message = "Employee, LeaveType, and Leave must belong to the same tenant." });

                if (employee.OrganizationId != null)
                    return BadRequest(new { Message = "Employee must not belong to any organization for tenant-only leave." });

                if (leaveType.OrganizationId != null)
                    return BadRequest(new { Message = "LeaveType is organization-specific but no OrganizationId provided." });
            }
            else
            {
                if (leaveType.TenantId != dto.TenantId ||
                    leaveType.OrganizationId != dto.OrganizationId ||
                    employee.TenantId != dto.TenantId ||
                    employee.OrganizationId != dto.OrganizationId)
                    return BadRequest(new { Message = "Employee, LeaveType, and Leave must match both TenantId and OrganizationId." });
            }

            // Define year range
            var yearStart = new DateTime(DateTime.UtcNow.Year, 1, 1);
            var yearEnd = new DateTime(DateTime.UtcNow.Year, 12, 31);

            var leaveQuery = _context.Leaves
                .Where(l =>
                    l.EmployeeId == employeeId &&
                    l.LeaveTypeId == dto.LeaveTypeId &&
                    l.StartDate >= yearStart &&
                    l.EndDate <= yearEnd &&
                    (l.Status == "Approved" || l.Status == "Pending"));

            if (dto.OrganizationId == null)
                leaveQuery = leaveQuery.Where(l => l.TenantId == dto.TenantId && l.OrganizationId == null);
            else
                leaveQuery = leaveQuery.Where(l => l.TenantId == dto.TenantId && l.OrganizationId == dto.OrganizationId);

            var usedDays = await leaveQuery
                .SumAsync(l => EF.Functions.DateDiffDay(l.StartDate, l.EndDate) + 1);

            var requestedDays = (dto.EndDate - dto.StartDate).Days + 1;
            var remainingDays = leaveType.MaxDays - usedDays;

            if (remainingDays < requestedDays)
            {
                return BadRequest(new
                {
                    Message = $"Insufficient {leaveType.Name} days remaining.",
                    UsedDays = usedDays,
                    RemainingDays = remainingDays,
                    RequestedDays = requestedDays,
                    MaxAllowed = leaveType.MaxDays
                });
            }

            // Create Leave request (do not subtract remainingDays yet)
            var leave = new Leave
            {
                EmployeeId = employeeId,
                LeaveTypeId = dto.LeaveTypeId,
                TenantId = employee.TenantId,
                OrganizationId = dto.OrganizationId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Reason = dto.Reason,
                Status = leaveType.RequiresApproval ? "Pending" : "Approved",
                AppliedOn = DateTime.UtcNow
            };

            await _context.Leaves.AddAsync(leave);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Leave request submitted successfully.",
                Leave = new
                {
                    LeaveId = leave.Id,
                    EmployeeName = $"{employee.FirstName} {employee.LastName}",
                    LeaveType = leaveType.Name,
                    duration = $"{Math.Round((leave.EndDate - leave.StartDate).TotalHours, 2)}hr",
                    RequestedDays = requestedDays,
                    RemainingDays = remainingDays,  // just show remaining, do NOT subtract yet
                    Status = leave.Status
                }
            });
        }



        [HttpGet("requests/employee")] // For leave requests of a single employee
        public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetEmployeeLeaveRequests(int employeeId)
        {
            var leaves = await _context.Leaves
                .Include(l => l.Employee)
                .Select(l => new
                {
                    EmployeeName = l.Employee != null
                        ? l.Employee.FirstName + " " + l.Employee.LastName
                        : "Unknown",
                    Status = l.Status ?? "Pending",
                    UpdatedAt = l.UpdatedAt,
                    ApprovedAgoHr = l.UpdatedAt.HasValue
                        ? $"{Math.Floor((DateTime.UtcNow - l.UpdatedAt.Value).TotalHours)}"
                        : "N/A"
                })
                .ToListAsync();

            return Ok(leaves);
        }


        // DELETE: api/leave/{id}
        [HttpDelete("{id}")]
        [RoleAuthorize("SuperAdmin , SystemAdmin , HR")]
        public async Task<IActionResult> DeleteLeaveRequest(Guid id)
        {
            var leave = await _context.Leaves.FindAsync(id);
            if (leave == null)
                return NotFound(new { Message = "Leave request not found." });

            _context.Leaves.Remove(leave);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Leave request deleted successfully." });
        }



        // Leave statistics by tenant only
        [HttpGet("stats/{tenantId}")]
        public async Task<IActionResult> GetLeaveStatsByTenant(Guid tenantId)
        {
            if (tenantId == Guid.Empty)
                return BadRequest("Tenant ID must be provided.");

            var leaves = await _context.Leaves
                .Include(l => l.Employee)
                .Include(l => l.LeaveType) // include LeaveType!
                .Where(l => l.Employee.TenantId == tenantId && l.OrganizationId == null)
                .AsNoTracking()
                .ToListAsync();

            if (!leaves.Any())
                return Ok(new { message = "No leave records found for this tenant." });

            var recentLeave = leaves.First();

            var leaveList = leaves.Select(l => new
            {
                leaveId = l.Id,
                employeeName = $"{l.Employee?.FirstName} {l.Employee?.LastName}".Trim(),
                leaveType = l.LeaveType?.Name ?? "N/A",
                durationDays = (l.EndDate - l.StartDate).Days + 1,
                reason = l.Reason,
                status = l.Status
            }).ToList();

            return Ok(new
            {
                message = "Leave request statistics retrieved successfully",
                leaves = leaveList,          // note plural
                totalRequests = leaves.Count,
                pendingRequests = leaves.Count(l => l.Status == "Pending"),
                approved = leaves.Count(l => l.Status == "Approved")
            });

        }

        // Leave statistics by tenant and organization
        [HttpGet("stats/{tenantId}/{organizationId}")]
        public async Task<IActionResult> GetLeaveStatsByTenantAndOrg(Guid tenantId, Guid organizationId)
        {
            if (tenantId == Guid.Empty)
                return BadRequest("Tenant ID must be provided.");
            if (organizationId == Guid.Empty)
                return BadRequest("Organization ID must be provided.");

            var leaves = await _context.Leaves
                .Include(l => l.Employee)
                .Include(l => l.LeaveType) // include LeaveType
                .Where(l => l.Employee.TenantId == tenantId && l.OrganizationId == organizationId)
                .AsNoTracking()
                .ToListAsync();

            if (!leaves.Any())
                return Ok(new { message = "No leave records found for this tenant and organization." });

            var leaveList = leaves.Select(l => new
            {
                leaveId = l.Id,
                employeeName = $"{l.Employee?.FirstName} {l.Employee?.LastName}".Trim(),
                OrganizationId = l.OrganizationId,
                leaveType = l.LeaveType?.Name ?? "N/A",
                durationDays = (l.EndDate - l.StartDate).Days + 1,
                reason = l.Reason,
                status = l.Status
            }).ToList();

            return Ok(new
            {
                message = "Leave request statistics retrieved successfully",
                leaves = leaveList,              // note plural
                totalRequests = leaves.Count,
                pendingRequests = leaves.Count(l => l.Status == "Pending"),
                approved = leaves.Count(l => l.Status == "Approved")
            });
        }


        //GET Info for employee portal
        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetLeavesByUser(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest("UserId is required.");

            // Find the user
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound(new { Message = "User not found." });

            if (user.EmployeeId == null)
                return BadRequest(new { Message = "User is not linked to an employee." });

            var employeeId = user.EmployeeId.Value;

            // Find the employee
            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeID == employeeId);

            if (employee == null)
                return NotFound(new { Message = "Employee not found." });

            // Get all leave types for this employee's tenant/org
            var leaveTypes = await _context.LeaveTypes
                .Where(lt => lt.TenantId == employee.TenantId &&
                             (lt.OrganizationId == null || lt.OrganizationId == employee.OrganizationId))
                .AsNoTracking()
                .ToListAsync();

            // Calculate total leave allowed
            int totalLeave = leaveTypes.Sum(lt => lt.MaxDays);

            // Get all leave requests for this employee in current year
            var yearStart = new DateTime(DateTime.UtcNow.Year, 1, 1);
            var yearEnd = new DateTime(DateTime.UtcNow.Year, 12, 31);

            var leaves = await _context.Leaves
                .Include(l => l.LeaveType)
                .Where(l => l.EmployeeId == employeeId &&
                            l.StartDate >= yearStart &&
                            l.EndDate <= yearEnd &&
                            (l.TenantId == employee.TenantId &&
                             (l.OrganizationId == null || l.OrganizationId == employee.OrganizationId)))
                .OrderByDescending(l => l.AppliedOn)
                .AsNoTracking()
                .ToListAsync();

            // Calculate used leave (sum of approved leave days)
            int usedLeave = leaves
                .Where(l => l.Status == "Approved")
                .Sum(l => (l.EndDate - l.StartDate).Days + 1);

            int remainingLeave = totalLeave - usedLeave;

            // Map leaves by status
            // Map leaves by status
            var leavesByStatus = leaves
                .GroupBy(l => l.Status ?? "Unknown")  // <-- coalesce null to "Unknown"
                .ToDictionary(
                    g => g.Key,  // now guaranteed non-null
                    g => g.Select(l => new
                    {
                        leaveId = l.Id,
                        Date = l.AppliedOn,
                        employeeName = $"{employee.FirstName} {employee.LastName}",
                        leaveType = l.LeaveType?.Name ?? "N/A",
                        duration = $"{Math.Round((l.EndDate - l.StartDate).TotalHours, 2)}hr",
                        reason = l.Reason,
                        status = l.Status ?? "Unknown"
                    }).ToList()
                );


            return Ok(new
            {
                EmployeeId = employee.EmployeeID,
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                TotalLeave = totalLeave,
                UsedLeave = usedLeave,
                RemainingLeave = remainingLeave,
                LeavesByStatus = leavesByStatus
            });
        }





    }
}
