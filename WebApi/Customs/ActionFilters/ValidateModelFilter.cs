using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using WebApi.Wrappers;

namespace WebApi.Customs.ActionFilters
{
    public class ValidateModelFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid) return;

            var errors = context.ModelState.Values.Where(v => v.Errors.Count > 0)
                .SelectMany(v => v.Errors)
                .Select(v => v.ErrorMessage)
                .ToList();

            var responseModel = new Response<List<string>>
            {
                Succeeded = false,
                Data = null,
                Errors = errors,
                Message = string.Empty
            };

            context.Result = new BadRequestObjectResult(responseModel)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
    }
}
