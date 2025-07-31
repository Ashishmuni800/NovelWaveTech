using Microsoft.AspNetCore.Authorization;

namespace NovelWaveTechUI.Filters
{
    public class CustomAccessHandler : AuthorizationHandler<CustomAccessRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomAccessRequirement requirement)
        {
            var httpContext = (context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext)?.HttpContext;

            // Example condition: Allow only from specific IP
            var remoteIp = httpContext?.Connection.RemoteIpAddress?.ToString();
            if (remoteIp == "127.0.0.1") // replace with your logic
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

}
