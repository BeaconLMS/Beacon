using Beacon.API;
using Beacon.API.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBeaconCore(builder.Configuration, options =>
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

app.MapBeaconEndpoints();
app.MapGet("api/ping", () => Results.Ok("pong"));

app.MapFallbackToFile("index.html");

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<BeaconDbContext>().Database.MigrateAsync();
}

app.Run();
