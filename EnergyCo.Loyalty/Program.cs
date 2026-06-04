using EnergyCo.Loyalty.Api.Middleware;
using EnergyCo.Loyalty.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(o =>
    {
        o.Filters.Add(new Microsoft.AspNetCore.Mvc.ProducesAttribute("application/json"));
    })
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

// Configure JWT bearer authentication for the API.
// In production, this would validate tokens issued by the chosen identity provider(e.g. Entra ID, Auth0).
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

// Allow anonymous access in Development to make local testing and demos easier.
// In other environments, require authentication by default across the API.
builder.Services.AddAuthorization(options =>
{
    if (!builder.Environment.IsDevelopment())
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
});
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
