using Beacon.Common;
using FluentValidation;
using FluentValidation.Results;
using System.Net;
using System.Net.Http.Json;

namespace BeaconUI.Core;

public static class HttpResponseMessageExtensions
{
    public static async Task<HttpResponseMessage> ValidateAsync(this HttpResponseMessage response, CancellationToken ct = default)
    {
        if (response.StatusCode is HttpStatusCode.UnprocessableEntity)
        {
            var problem = await response.Content.ReadFromJsonAsync<BeaconValidationProblem>(cancellationToken: ct);
            var failures = problem?.Errors.SelectMany(e => e.Value.Select(v => new ValidationFailure(e.Key, v)));
            throw new ValidationException(failures);
        }

        response.EnsureSuccessStatusCode();
        return response;
    }

    public static async Task<T> ValidateAsync<T>(this HttpResponseMessage response, CancellationToken ct = default)
    {
        await response.ValidateAsync(ct);
        var content = await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
        return content ?? throw new Exception("There was an unexpected problem deserializing the response.");
    }
}
