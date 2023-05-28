using Beacon.API;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

services.AddAuthentication()
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

services.AddAuthorization();

services.AddBeaconCore(options =>
{
    var connectionString = config.GetConnectionString("SqlServerDb");
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();
app.MapGroup("api").UseBeaconCore();

app.MapFallbackToFile("index.html");
app.Run();
