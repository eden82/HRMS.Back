namespace HRMS.Backend.DTOs
{
    public class UpdatePasswordDto
    {
        public Guid UserId { get; set; } // or string Email if you prefer
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
