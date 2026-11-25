using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ParcelTrack.Filters
{
    public class RequireClientAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string? client = context.HttpContext.Request.Headers["X-Client"];

            if (string.IsNullOrWhiteSpace(client))
            {
                context.Result = new BadRequestObjectResult(new
                {
                    message = "Say which program is calling: add an X-Client header"
                });
                return;
            }

            string? actionName = context.ActionDescriptor.RouteValues["action"];
            Console.WriteLine($"X-Client: {client} -> {actionName}");
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            string? actionName = context.ActionDescriptor.RouteValues["action"];
            Console.WriteLine($"{actionName} finished");
        }
    }
}
