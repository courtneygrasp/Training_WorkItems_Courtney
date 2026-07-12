using grasp.Infrastructure.Sso.OAuth.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Training.WorkItems.Api.Auth;
using Training.WorkItems.Api.CompositionRoot;
using Training.WorkItems.Core.DependencyInjection;

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapSsoEndpoints();

app.Run();
