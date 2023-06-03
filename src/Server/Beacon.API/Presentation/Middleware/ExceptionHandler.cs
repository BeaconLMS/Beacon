using Beacon.Common.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Beacon.API.Presentation.Middleware;

public static class ExceptionHandler
{
    public static IResult HandleException(Exception ex)
    {
        if (ex is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).Distinct().ToArray());

            return Results.UnprocessableEntity(new BeaconValidationProblem { Errors = errors });
        }

        return Results.Problem("An unexpected error has occurred.", statusCode: 500);
    }
}
