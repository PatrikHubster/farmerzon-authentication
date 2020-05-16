using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Authentication.Helper
{
    // This class is implemented like explained on the following website:
    // https://www.talkingdotnet.com/validate-model-state-automatically-asp-net-core-2-0/
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    Success = false,
                    Errors = context.ModelState.Values.SelectMany(
                        value => value.Errors.Select(
                            message => message.ErrorMessage)).ToList()
                });
            }
        }
    }
}