using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NovelWaveTechUI.Filters
{
    public class CustomRestrictionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Custom logic: deny based on time, headers, etc.
            var isAllowed = false;

            // Example: deny access on weekends
            if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday &&
                DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
            {
                isAllowed = true;
            }

            if (!isAllowed)
            {
                context.Result = new ContentResult
                {
                    StatusCode = 403,
                    Content = "Access denied"
                };
            }
        }
    }

}
