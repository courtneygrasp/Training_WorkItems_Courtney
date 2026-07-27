using grasp.Infrastructure.Sso.OAuth.Authorization;
using grasp.Infrastructure.Sso.OAuth.TokenExchange;

namespace Training.WorkItems.Api.Auth;

public static class MapSsoEndpointExtensions
{
    public static void MapSsoEndpoints(this WebApplication app)
    {
        app.MapGet("/auth/start", StartAuth)
            .AllowAnonymous();

        app.MapGet("/auth/signin", SignIn)
            .AllowAnonymous();
    }

    private static IResult StartAuth(
        HttpContext httpContext,
        IOAuthAuthorizationUseCase authorizationUseCase,
        string? tenant)
    {
        var authorizationRedirect = authorizationUseCase.Execute(
            httpContext,
            new OAuthAuthorizationRequest(tenant, null));

        return Results.Redirect(authorizationRedirect.AuthorizeUrl);
    }

    private static async Task<IResult> SignIn(
        HttpContext httpContext,
        IOAuthTokenExchangeUseCase tokenExchangeUseCase,
        ILocalSignInService localSignInService,
        CancellationToken cancellationToken)
    {
        var code = httpContext.Request.Query["code"].ToString();
        var state = httpContext.Request.Query["state"].ToString();

        var tokenResponse = await tokenExchangeUseCase.ExecuteAsync(
            httpContext,
            new OAuthTokenExchangeRequest(code, state),
            cancellationToken);

        var signInResult = await localSignInService.SignInAsync(
            httpContext,
            tokenResponse,
            cancellationToken);

        return signInResult.RedirectToHome
            ? Results.Redirect("/api/workitems")
            : Results.Redirect("/auth/start");
    }
}
