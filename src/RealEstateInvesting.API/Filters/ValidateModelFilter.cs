using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RealEstateInvesting.API.Contracts;

namespace RealEstateInvesting.API.Filters;

public class ValidateModelFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .SelectMany(x => x.Value!.Errors)
                .Select(e => new ApiError
                {
                    Code = "VALIDATION_ERROR",
                    Message = e.ErrorMessage
                })
                .ToList();

            context.Result = new BadRequestObjectResult(
                ApiResponse<object>.Failure(
                    "Validation failed",
                    400,
                    errors));
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
