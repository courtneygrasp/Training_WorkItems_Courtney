using System.Data.Common;
using grasp.Infrastructure.Observability.Correlation.DependencyInjection;
using grasp.Infrastructure.Sso.OAuth.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Training.WorkItems.Api.Auth;
using Training.WorkItems.Api.CompositionRoot;
using Training.WorkItems.Core.DependencyInjection;

DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.AddGraspSecurity();
builder.Services.AddOAuthSsoFeature();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/start";
        options.LogoutPath = "/auth/logout";
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<ILocalSignInService, LocalSignInService>();
builder.Services.AddTrainingWorkItemsCore(builder.Configuration);

builder.Services.AddGraspCorrelation();

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionFeature?.Error is { } error)
        {
            logger.LogError(error, "Unhandled exception processing request.");
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseGraspCorrelation();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapSsoEndpoints();

app.Run();

public partial class Program { }
