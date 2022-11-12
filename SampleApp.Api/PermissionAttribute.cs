using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace SampleApp.Api
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PermissionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            dynamic bodyData = context.ActionArguments.Values.First();
            string stage = bodyData.Stage;
            var roles = context.HttpContext.User.Claims.Where(x => x.Type== "role");
            if (!roles.Any(x => x.Value.ToLower() == stage.ToLower()))
                context.Result = new ForbidResult(JwtBearerDefaults.AuthenticationScheme);

        }
    }
}
