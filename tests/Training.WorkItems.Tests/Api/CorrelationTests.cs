using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using AwesomeAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Application.WorkItems.Services;
using Training.WorkItems.Infrastructure.WorkItems.Services;
using Training.WorkItems.Tests.Application.WorkItems.Fakes;

namespace Training.WorkItems.Tests.Api;

public sealed class CorrelationTests(CorrelationWebApplicationFactory factory)
    : IClassFixture<CorrelationWebApplicationFactory>
{
    [Fact]
    public async Task Request_Always_IncludesCorrelationIdResponseHeader()
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/api/workitems");

        response.Headers.Contains("X-Grasp-Correlation-Id").Should().BeTrue();
    }

    [Fact]
    public async Task Request_WithValidInboundCorrelationId_PreservesItInResponse()
    {
        // Grasp correlation IDs are 32-char lowercase hex (UUID without dashes)
        var inboundId = Guid.NewGuid().ToString("N");
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        client.DefaultRequestHeaders.Add("X-Grasp-Correlation-Id", inboundId);

        var response = await client.GetAsync("/api/workitems");

        var responseId = response.Headers.GetValues("X-Grasp-Correlation-Id").Single();
        responseId.Should().Be(inboundId);
    }

    [Fact]
    public async Task Request_WithInvalidInboundCorrelationId_GeneratesNewValidId()
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        client.DefaultRequestHeaders.Add("X-Grasp-Correlation-Id", "not-a-valid-grasp-id");

        var response = await client.GetAsync("/api/workitems");

        // Grasp correlation IDs are 32-char lowercase hex (UUID without dashes)
        var responseId = response.Headers.GetValues("X-Grasp-Correlation-Id").Single();
        responseId.Should().NotBe("not-a-valid-grasp-id");
        responseId.Should().HaveLength(32);
        responseId.Should().MatchRegex("^[0-9a-f]{32}$");
    }
}

public sealed class CorrelationWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services
                .AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, AlwaysAuthenticatedHandler>("Test", _ => { });

            services.Configure<AuthenticationOptions>(options =>
            {
                options.DefaultScheme = "Test";
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            });

            services.AddScoped<ICurrentUserContext, DefaultCurrentUserContext>();
            services.AddScoped<IWorkItemRepository>(_ => new InMemoryWorkItemRepository());
        });
    }
}

public sealed class AlwaysAuthenticatedHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim(ClaimTypes.NameIdentifier, "22222222-2222-2222-2222-222222222222"),
            new Claim("tenant_id", "11111111-1111-1111-1111-111111111111"),
            new Claim("can_close_work_items", "true"),
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
