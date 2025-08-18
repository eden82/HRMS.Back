
namespace HRMS.Backend.DTOs
{
	public class CreateUserRequest
	{
		public string FullName { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string PhoneNumber { get; set; } = null!;
		public string Password { get; set; } = null!;
		public string Role { get; set; } = null!;

		// Add this property
		public string? Username { get; set; }

		public int? OrganizationId { get; set; } // null for SuperAdmin
	}


}