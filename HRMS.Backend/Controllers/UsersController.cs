using System;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Backend.Data;
using HRMS.Backend.DTOs;
using HRMS.Backend.Models;
using HRMS.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS.Backend.Filters;
using Microsoft.EntityFrameworkCore.Storage;


namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[RoleAuthorize("SuperAdmin,SystemAdmin,HR")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher _hasher;
        private readonly EmailService _emailService;


        public UsersController(AppDbContext db, IPasswordHasher hasher, EmailService emailService)
        {
            _db = db;
            _hasher = hasher;
            _emailService = emailService;

        }

        public sealed class CreateUserDto
        {
            public string FullName { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
            public string Password { get; set; } = string.Empty;
            public string Role { get; set; } = "User";
            public Guid? TenantId { get; set; }
            public Guid? OrganizationId { get; set; }
            public Guid? EmployeeId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto input)
        {
            
            if (string.IsNullOrWhiteSpace(input.Password)) return BadRequest("Password is required.");

            var normalizedEmail = string.IsNullOrWhiteSpace(input.Email) ? null : input.Email!.Trim().ToUpperInvariant();


            if (normalizedEmail != null)
            {
                var emailTaken = await _db.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail);
                if (emailTaken) return Conflict("Email already in use.");
            }

            _hasher.Create(input.Password, out var hash, out var salt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = input.FullName.Trim(),
                Email = input.Email,
                NormalizedEmail = normalizedEmail,
                PhoneNumber = input.PhoneNumber,
                PasswordHash = hash,
                PasswordSalt = salt,
                TenantId = input.TenantId,
                OrganizationId = input.OrganizationId,
                EmployeeId = input.EmployeeId,
                SecurityStamp = Guid.NewGuid().ToString("N"),
                CreatedAt = DateTime.UtcNow,
                UserRoles = new List<UserRole>() //  initialize here
            };

            // Add role mapping
            var roleId = await _db.Roles
                .Where(r => r.Name.ToUpper() == input.Role.ToUpper()) // case-insensitive
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (roleId == Guid.Empty)
            {
                return BadRequest($"Role '{input.Role}' does not exist.");
            }

            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = roleId
            });

            _db.Users.Add(user);
            await _db.SaveChangesAsync();



            //  Send the password email here
            try
            {
                var message = $"Welcome {user.FullName},\n\n" +
                              $"Your HRMS account has been created.\n\n" +
                              $"Login details:\n" +
                              $"Email: {user.Email}\n" +
                              $"Password: {input.Password}\n\n" +
                              $"Please keep this information secure.\n\n" +
                              $"Best regards,\nHRMS Team";

                await _emailService.SendEmailAsync(user.Email!, "Welcome to HRMS", message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                // Optional: log or handle gracefully, but don’t fail registration
            }

            return CreatedAtAction(nameof(GetById), new { id = user.Id },
                new { user.Id, user.FullName, user.Email, Role = input.Role });
        }

        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto input)
        {
            if (string.IsNullOrWhiteSpace(input.NewPassword))
                return BadRequest("New password cannot be empty.");

            var user = await _db.Users.FindAsync(input.UserId);
            if (user == null)
                return NotFound("User not found.");

            // Verify the old password first
            var isValid = _hasher.Verify(input.OldPassword, user.PasswordHash, user.PasswordSalt);
            if (!isValid)
                return BadRequest("Old password is incorrect.");

            // Hash new password
            _hasher.Create(input.NewPassword, out var newHash, out var newSalt);

            user.PasswordHash = newHash;
            user.PasswordSalt = newSalt;
            user.SecurityStamp = Guid.NewGuid().ToString("N");
            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            // Send confirmation email
            try
            {
                var subject = "HRMS Password Updated";
                var message = $"Hello {user.FullName},\n\n" +
                              $"Your password has been successfully updated.\n\n" +
                              $"Password: {input.NewPassword}\n\n" +
                              $"If this wasn’t you, please contact support immediately.\n\n" +
                              $"Best regards,\nHRMS Team";

                await _emailService.SendEmailAsync(user.Email!, subject, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email failed: {ex.Message}");
            }

            return Ok("Password updated successfully.");
        }



        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _db.Users
                .AsNoTracking()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.PhoneNumber,
                Roles = user.UserRoles.Select(ur => ur.Role!.Name).ToList(),
                user.TenantId,
                user.OrganizationId,
                user.EmployeeId,
                user.IsActive,
                user.LastLoginUtc,
                user.CreatedAt,
                user.UpdatedAt
            });
        }


        // Example where the old error happened – ensure we use LastLoginUtc (not LastLoginAtUtc)
        [HttpPost("{id:guid}/touch-login")]
        public async Task<IActionResult> TouchLogin(Guid id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return NotFound();

            u.LastLoginUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(new
            {
                u.LastLoginUtc
            });
        }



        [HttpGet("superadmins")]
        public async Task<IActionResult> GetSuperAdmins()
        {
            var superAdmins = await _db.Users
                .AsNoTracking()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => u.UserRoles.Any(ur => ur.Role!.Name == "SuperAdmin"))
                .Select(user => new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.PhoneNumber,
                    Roles = user.UserRoles.Select(ur => ur.Role!.Name).ToList(),
                    user.IsActive,
                    user.LastLoginUtc,
                    user.CreatedAt
                })
                .ToListAsync();

            return Ok(superAdmins);
        }



        [HttpGet("systemadmin/{tenantId}")]
        public async Task<IActionResult> GetTenantSystemAdmin(Guid tenantId)
        {
            var superAdmin = await _db.Users
                .AsNoTracking()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u =>
                    u.TenantId == tenantId &&
                    u.UserRoles.Any(ur => ur.Role!.Name == "SystemAdmin"))
                .Select(user => new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.PhoneNumber,
                    Roles = user.UserRoles.Select(ur => ur.Role!.Name).ToList(),
                    user.IsActive,
                    user.LastLoginUtc,
                    user.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (superAdmin == null)
                return NotFound($"No SystemAdmin found for tenant {tenantId}");

            return Ok(superAdmin);
        }




        [HttpGet("by-tenant/{tenantId:guid}")]
        public async Task<IActionResult> GetUsersByTenantId(Guid tenantId)
        {
            var users = await _db.Users
                .AsNoTracking()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => u.TenantId == tenantId)
                .Select(user => new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.PhoneNumber,
                    Roles = user.UserRoles.Select(ur => ur.Role!.Name).ToList(),
                    user.IsActive,
                    user.LastLoginUtc,
                    user.CreatedAt
                })
                .ToListAsync();


            return Ok(users);
        }

        [HttpGet("search-employee-by-email")]
    public async Task<IActionResult> SearchEmployeeByEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email is required.");

        // Normalize the email for case-insensitive search
        var normalizedEmail = email.Trim().ToUpperInvariant();

        var employee = await _db.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Email.ToUpper() == normalizedEmail);

        if (employee == null)
            return NotFound($"No employee found with email: {email}");

        return Ok(new
        {
            employee.EmployeeID,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.PhoneNumber,
            employee.JobTitle,
            employee.DepartmentId,
            employee.OrganizationId,
            employee.TenantId
        });
    }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return NoContent();
        }



        [HttpPost("forgot-password/request-otp")]
        public async Task<IActionResult> RequestPasswordOtp([FromBody] RequestPasswordOtpDto input)
        {
            var normalizedEmail = input.Email.Trim().ToUpperInvariant();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

            if (user == null)
                return NotFound("User with this email not found.");

            // Generate OTP (6 digits)
            var otp = new Random().Next(100000, 999999).ToString();
            user.PasswordResetOtp = otp;
            user.PasswordResetOtpExpires = DateTime.UtcNow.AddMinutes(5);

            await _db.SaveChangesAsync();

            // Send email
            var subject = "Your HRMS Password Reset OTP";
            var body = $"Hello {user.FullName},\n\n" +
                       $"Your OTP for password reset is: {otp}\n" +
                       $"This code expires in 5 minutes.\n\n" +
                       $"If you didn’t request this, ignore this email.\n\n" +
                       $"– HRMS Team";

            await _emailService.SendEmailAsync(user.Email!, subject, body);

            return Ok("OTP sent to your email.");
        }

        [HttpPost("forgot-password/verify-otp")]
        public async Task<IActionResult> VerifyPasswordOtp([FromBody] VerifyOtpDto input)
        {
            var normalizedEmail = input.Email.Trim().ToUpperInvariant();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

            if (user == null)
                return NotFound("User not found.");

            if (user.PasswordResetOtp != input.Otp)
                return BadRequest("Invalid OTP.");

            if (user.PasswordResetOtpExpires < DateTime.UtcNow)
                return BadRequest("OTP has expired.");

            return Ok("OTP verified successfully. You can now reset your password.");
        }

        [HttpPost("forgot-password/change")]
        public async Task<IActionResult> ChangePasswordWithOtp([FromBody] ChangePasswordWithOtpDto input)
        {
            var normalizedEmail = input.Email.Trim().ToUpperInvariant();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

            if (user == null)
                return NotFound("User not found.");

            if (user.PasswordResetOtp != input.Otp)
                return BadRequest("Invalid OTP.");

            if (user.PasswordResetOtpExpires < DateTime.UtcNow)
                return BadRequest("OTP expired.");

            // Hash new password (no need for old password)
            _hasher.Create(input.NewPassword, out var hash, out var salt);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            //  Clear OTP fields
            user.PasswordResetOtp = null;
            user.PasswordResetOtpExpires = null;

            user.SecurityStamp = Guid.NewGuid().ToString("N");
            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            //  Send confirmation email
            var subject = "HRMS Password Reset Successful";
            var body = $"Hello {user.FullName},\n\n" +
                       $"Your password has been successfully reset.\n" +
                       $"Password: {input.NewPassword}\n\n" +
                       $"If this wasn’t you, contact support immediately.\n\n" +
                       $"– HRMS Team";

            await _emailService.SendEmailAsync(user.Email!, subject, body);

            return Ok("Password reset successfully.");
        }






    }
}
