﻿using Beacon.Common.Responses;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Beacon.WebApp.IntegrationTests;

public static class MockHttpHelper
{
    public static MockHttpMessageHandler AddMockHttpClient(this TestServiceProvider services)
    {
        var mockHttpHandler = new MockHttpMessageHandler();

        var httpClient = mockHttpHandler.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost");
        services.AddSingleton(httpClient);

        return mockHttpHandler;
    }

    public static MockedRequest ThenRespondOK<T>(this MockedRequest request, T content)
    {
        return request.Respond(_ => CreateResponse(HttpStatusCode.OK, content));
    }

    public static MockedRequest ThenRespondValidationProblem(this MockedRequest request, Dictionary<string, string[]> errors)
    {
        return request.Respond(_ => CreateResponse(HttpStatusCode.UnprocessableEntity, new ValidationProblemResponse { Errors = errors }));
    }

    private static HttpResponseMessage CreateResponse<T>(HttpStatusCode statusCode, T content)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(JsonSerializer.Serialize(content))
        };

        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return response;
    }
}