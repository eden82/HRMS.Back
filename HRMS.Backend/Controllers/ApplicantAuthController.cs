using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS.Backend.Data;
using HRMS.Backend.Models;
using HRMS.Backend.DTOs;
using HRMS.Backend.Services;
using HRMS.Backend.Filters;
using Microsoft.EntityFrameworkCore.Storage;



[ApiController]
[Route("api/[controller]")]
public class ApplicantAuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;
    private readonly EmailService _email;

    public ApplicantAuthController(AppDbContext db, IPasswordHasher hasher, IJwtTokenService jwt, EmailService email)
    {
        _db = db;
        _hasher = hasher;
        _jwt = jwt;
        _email = email;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] ApplicantRegisterRequest input)
    {
        if (await _db.ApplicantRegistrations.AnyAsync(a => a.Email.ToUpper() == input.Email.Trim().ToUpper()))
            return Conflict("Email already exists.");

        _hasher.Create(input.Password, out var hash, out var salt);

        var applicant = new ApplicantRegistration
        {
            Id = Guid.NewGuid(),
            Fullname = input.Fullname,
            Email = input.Email,
            //Phone = input.Phone,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        _db.ApplicantRegistrations.Add(applicant);
        await _db.SaveChangesAsync();

        return Created("", new
        {
            applicant.Id,
            applicant.Fullname,
            applicant.Email
            //applicant.Phone
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] ApplicantLoginRequest input)
    {
        var emailNorm = input.Email.Trim().ToUpper();
        var applicant = await _db.ApplicantRegistrations
            .FirstOrDefaultAsync(a => a.Email.ToUpper() == emailNorm);

        if (applicant == null)
            return Unauthorized("Invalid email or password.");

        if (!_hasher.Verify(input.Password, applicant.PasswordHash, applicant.PasswordSalt))
            return Unauthorized("Invalid email or password.");

        // Generate OTP
        var otp = new Random().Next(100000, 999999).ToString();
        applicant.OtpCode = otp;
        applicant.OtpExpiryUtc = DateTime.UtcNow.AddMinutes(5);
        await _db.SaveChangesAsync();

        await _email.SendOtpAsync(applicant.Email, otp);

        return Ok(new ApplicantLoginResponse
        {
            Id = applicant.Id.ToString(),
            Email = applicant.Email,
            Fullname = applicant.Fullname,
            Message = "OTP sent to your email.",
            RequiresOtp = true
        });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyRequest input)
    {
        var emailNorm = input.UsernameOrEmail.Trim().ToUpper();
        var applicant = await _db.ApplicantRegistrations
            .FirstOrDefaultAsync(a => a.Email.ToUpper() == emailNorm);

        if (applicant == null)
            return Unauthorized("Invalid email.");

        if (applicant.OtpCode != input.OtpCode || applicant.OtpExpiryUtc < DateTime.UtcNow)
            return Unauthorized("OTP invalid or expired.");

        // Clear OTP
        applicant.OtpCode = null;
        applicant.OtpExpiryUtc = null;
        await _db.SaveChangesAsync();

        // Generate JWT specifically for applicants
        var (jwt, exp, _) = await _jwt.CreateAccessTokenForApplicantAsync(applicant);
        var (rt, rtExp) = _jwt.CreateRefreshToken();

        return Ok(new ApplicantLoginResponse
        {
            Id = applicant.Id.ToString(),
            AccessToken = jwt,
            ExpiresAt = exp,
            RefreshToken = rt,
            RefreshExpiresAt = rtExp,
            Fullname = applicant.Fullname,
            Email = applicant.Email,
            Message = "Login successful.",
            RequiresOtp = false,
            OtpVerified = true
        });
    }

    [HttpPut("update-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdateApplicantPasswordDto input)
    {
        if (string.IsNullOrWhiteSpace(input.NewPassword))
            return BadRequest("New password cannot be empty.");


        var applicant = await _db.ApplicantRegistrations.FindAsync(input.applicantId);
        if (applicant == null)
            return NotFound("applicant not found.");

        // Verify the old password first
        var isValid = _hasher.Verify(input.OldPassword, applicant.PasswordHash, applicant.PasswordSalt);
        if (!isValid)
            return BadRequest("Old password is incorrect.");

        // Hash new password
        _hasher.Create(input.NewPassword, out var newHash, out var newSalt);

        applicant.PasswordHash = newHash;
        applicant.PasswordSalt = newSalt;
        applicant.SecurityStamp = Guid.NewGuid().ToString("N");
        applicant.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        // Send confirmation email
        try
        {
            var subject = "HRMS Password Updated";
            var message = $"Hello {applicant.Fullname},\n\n" +
                          $"Your password has been successfully updated.\n\n" +
                          $"Password: {input.NewPassword}\n\n" +
                          $"If this wasn’t you, please contact support immediately.\n\n" +
                          $"Best regards,\nHRMS Team";

            await _email.SendEmailAsync(applicant.Email!, subject, message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email failed: {ex.Message}");
        }

        return Ok("Password updated successfully.");
    }



    [HttpPut("update-Name")]
    public async Task<IActionResult> UpdateName([FromBody] UpdateApplicantNameDto input)
    {

        var applicant = await _db.ApplicantRegistrations.FindAsync(input.applicantId);
        if (applicant == null)
            return NotFound("applicant not found.");


        applicant.Fullname = input.FullName;

        await _db.SaveChangesAsync();

        // Send confirmation email
        try
        {
            var subject = "HRMS Full Name Updated";
            var message = $"Hello {applicant.Fullname},\n\n" +
                          $"Your Name has been successfully updated.\n\n" +
                          $"If this wasn’t you, please contact support immediately.\n\n" +
                          $"Best regards,\nHRMS Team";

            await _email.SendEmailAsync(applicant.Email!, subject, message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email failed: {ex.Message}");
        }

        return Ok("Name updated successfully.");
    }


    [HttpPost("forgot-password/request-otp")]
    public async Task<IActionResult> RequestPasswordOtp([FromBody] RequestPasswordOtpDto input)
    {
        var emailNorm = input.Email.Trim().ToUpper();
        var applicant = await _db.ApplicantRegistrations
            .FirstOrDefaultAsync(a => a.Email.ToUpper() == emailNorm);

        if (applicant == null)
            return NotFound("Applicant with this email not found.");

        // Generate OTP (6 digits)
        var otp = new Random().Next(100000, 999999).ToString();
        applicant.OtpCode = otp;
        applicant.OtpExpiryUtc = DateTime.UtcNow.AddMinutes(5);

        await _db.SaveChangesAsync();

        // Send email
        var subject = "Your HRMS Password Reset OTP";
        var body = $"Hello {applicant.Fullname},\n\n" +
                   $"Your OTP for password reset is: {otp}\n" +
                   $"This code expires in 5 minutes.\n\n" +
                   $"If you didn’t request this, ignore this email.\n\n" +
                   $"– HRMS Team";

        await _email.SendEmailAsync(applicant.Email!, subject, body);

        return Ok("OTP sent to your email.");
    }


    [HttpPost("forgot-password/verify-otp")]
    public async Task<IActionResult> VerifyPasswordOtp([FromBody] VerifyOtpDto input)
    {
        var emailNorm = input.Email.Trim().ToUpper();
        var applicant = await _db.ApplicantRegistrations
            .FirstOrDefaultAsync(a => a.Email.ToUpper() == emailNorm);

        if (applicant == null)
            return NotFound("Applicant not found.");

        if (applicant.OtpCode != input.Otp)
            return BadRequest("Invalid OTP.");

        if (applicant.OtpExpiryUtc < DateTime.UtcNow)
            return BadRequest("OTP has expired.");

        return Ok("OTP verified successfully. You can now reset your password.");
    }


    [HttpPost("forgot-password/change")]
    public async Task<IActionResult> ChangePasswordWithOtp([FromBody] ChangePasswordWithOtpDto input)
    {
        var emailNorm = input.Email.Trim().ToUpper();
        var applicant = await _db.ApplicantRegistrations
            .FirstOrDefaultAsync(a => a.Email.ToUpper() == emailNorm);

        if (applicant == null)
            return NotFound("Applicant not found.");

        if (applicant.OtpCode != input.Otp)
            return BadRequest("Invalid OTP.");

        if (applicant.OtpExpiryUtc < DateTime.UtcNow)
            return BadRequest("OTP expired.");

        // Hash new password
        _hasher.Create(input.NewPassword, out var hash, out var salt);
        applicant.PasswordHash = hash;
        applicant.PasswordSalt = salt;

        // Clear OTP fields
        applicant.OtpCode = null;
        applicant.OtpExpiryUtc = null;

        applicant.SecurityStamp = Guid.NewGuid().ToString("N");
        applicant.UpdatedAt = DateTime.UtcNow;
        
        await _db.SaveChangesAsync();

        // Send confirmation email
        var subject = "HRMS Password Reset Successful";
        var body = $"Hello {applicant.Fullname},\n\n" +
                   $"Your password has been successfully reset.\n" +
                   $"Password: {input.NewPassword}\n\n" +
                   $"If this wasn’t you, contact support immediately.\n\n" +
                   $"– HRMS Team";

        await _email.SendEmailAsync(applicant.Email!, subject, body);

        return Ok("Password reset successfully.");
    }



}
