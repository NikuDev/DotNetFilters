using DotNetFilters.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace DotNetFilters.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case ArgumentException:
                    context.Result = new BadRequestResult();
                    break;
                case InvalidOperationException:
                    context.Result = new NotFoundResult();
                    break;
                default:
                    break;
            }
        }
    }
}






var message = "A unexpected error occured";

if (context.Exception is PostalCodeNotAllowedException)
{
    message = $"Postalcode '{context.Exception.Message}' is not in our deliverable range";
}
if (context.Exception is CityNotAllowedException)
{
    message = $"City '{context.Exception.Message}' is not allowed";
}

var result = new BadRequestObjectResult(message);
context.Result = result;