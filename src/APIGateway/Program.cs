using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Headers;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);
var (config, services) = (builder.Configuration, builder.Services);

services
    .AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme);
services
    .AddAuthorizationBuilder();

services
    .AddHttpClient()
    .AddDataProtection();

services
    .AddReverseProxy()
    .LoadFromConfig(config.GetSection("ReverseProxy"))
    .AddTransforms(b =>
    {
        b.AddRequestTransform(async context =>
        {
            var accessToken = await context.HttpContext.GetTokenAsync("access_token");

            if (accessToken != null)
                context.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        });
    });

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapReverseProxy();

app.Run();
