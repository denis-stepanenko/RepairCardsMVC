using Microsoft.AspNetCore.Mvc.Filters;
using RepairCardsMVC.Exceptions;

namespace RepairCardsMVC.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is NotFoundException)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                context.ExceptionHandled = true;
            }
        }
    }
}
