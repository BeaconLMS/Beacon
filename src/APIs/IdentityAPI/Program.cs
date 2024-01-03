using IdentityAPI.Database;
using IdentityAPI.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var (config, services) = (builder.Configuration, builder.Services);

// API
services.AddEndpointsApiExplorer();

// Auth & Identity
services
    .AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme);
services
    .AddAuthorizationBuilder();
services
    .AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<IdentityDatabaseContext>()
    .AddDefaultTokenProviders()
    .AddApiEndpoints();

// Database
services.AddDbContext<IdentityDatabaseContext>(o =>
{
    o.UseSqlServer(config.GetConnectionString("IdentityDb"));
});

// Email
services.Configure<EmailSettings>(builder.Configuration.GetRequiredSection("EmailSettings"));
services.AddTransient<IEmailSender, EmailSender>();

// Swagger
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Beacon Identity API",
        Version = "v1"
    });

    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token.",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapIdentityApi<IdentityUser>();
app.MapGet("ping", () => "pong");

app.Run();
