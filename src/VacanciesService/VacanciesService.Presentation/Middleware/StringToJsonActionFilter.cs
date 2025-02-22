using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VacanciesService.Presentation.Middleware
{
    public class StringToJsonActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if(context.Result is ObjectResult objectResult && objectResult.Value is string stringValue)
            {
                context.Result = new JsonResult(stringValue);
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
