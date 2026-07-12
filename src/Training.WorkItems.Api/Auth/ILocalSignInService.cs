using grasp.Infrastructure.Sso.OAuth.TokenExchange;
using Microsoft.AspNetCore.Http;

namespace Training.WorkItems.Api.Auth;

public interface ILocalSignInService
{
    Task<LocalSignInResult> SignInAsync(
        HttpContext httpContext,
        OAuthTokenExchangeResponse tokenResponse,
        CancellationToken cancellationToken);
}
