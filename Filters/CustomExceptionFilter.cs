using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotNetFilters.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {

        public void OnException(ExceptionContext context)
        {
            var result = new BadRequestObjectResult("Custom error message.");
            context.Result = result;
        }
    }
}
