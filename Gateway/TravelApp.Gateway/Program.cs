using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Ensure Ocelot configuration is loaded
builder.Configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// JWT bearer so Ocelot can validate tokens on protected routes
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role
        };
    });

// Register Ocelot and Polly
builder.Services.AddOcelot(builder.Configuration)
    .AddPolly();

// Add CORS policies so Angular (localhost:4200) can make requests
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("CorsPolicy");

app.UseAuthentication();

// Start Ocelot pipeline
await app.UseOcelot();

app.Run();
