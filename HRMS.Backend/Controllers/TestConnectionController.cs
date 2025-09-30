using Microsoft.AspNetCore.Mvc;

namespace HRMS.Backend.Controllers
{
	[ApiController]
	[Route("api")]
	public class TestConnectionController : ControllerBase
	{
		[HttpGet("test")]
		public IActionResult Test()
		{
			return Ok(new { message = "API is working!" });
		}
	}
}
