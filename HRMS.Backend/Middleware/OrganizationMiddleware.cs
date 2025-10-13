using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HRMS.Backend.Middleware
{
    public class OrganizationMiddleware
    {
        private readonly RequestDelegate _next;

        public OrganizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Organization-ID", out var orgId))
            {
                // Store the organization ID in HttpContext.Items
                context.Items["OrganizationId"] = orgId.ToString();
            }

            await _next(context);
        }
    }
}
