using Microsoft.AspNetCore.Authorization;

namespace NovelWaveTechUI.Filters
{
    public class CustomAccessRequirement : IAuthorizationRequirement
    {
        public CustomAccessRequirement() { }
    }

}
