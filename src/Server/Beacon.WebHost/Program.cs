using Beacon.API;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBeaconCore(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SqlServerDb");
    options.UseSqlServer(connectionString);
});

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler(builder =>
{
    builder.Run(async context => await Results.Problem(statusCode: 422).ExecuteAsync(context));
});

var endpointRoot = app.MapGroup("api");
endpointRoot.MapBeaconEndpoints();
endpointRoot.MapGet("ping", () => Results.Ok("pong"));

app.MapFallbackToFile("index.html");
app.Run();
