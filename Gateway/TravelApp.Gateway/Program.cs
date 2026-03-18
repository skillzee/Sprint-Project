using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

// Ensure Ocelot configuration is loaded
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

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

// Start Ocelot pipeline
await app.UseOcelot();

app.Run();
