using System.Security.Claims;
using grasp.Infrastructure.Sso.OAuth.TokenExchange;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Training.WorkItems.Api.Auth;

public sealed class LocalSignInService : ILocalSignInService
{
    public async Task<LocalSignInResult> SignInAsync(
        HttpContext httpContext,
        OAuthTokenExchangeResponse tokenResponse,
        CancellationToken cancellationToken)
    {
        if (!tokenResponse.Validated)
        {
            return new LocalSignInResult(RedirectToHome: false);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, tokenResponse.Subject ?? tokenResponse.User ?? string.Empty),
            new(ClaimTypes.Name, tokenResponse.Name ?? tokenResponse.PreferredUsername ?? string.Empty),
            new(ClaimTypes.Email, tokenResponse.Email ?? string.Empty),
        };

        // Carry through all validated token claims so HttpContextCurrentUserContext can read them.
        foreach (var claim in tokenResponse.Claims)
        {
            if (claims.All(c => c.Type != claim.Type))
            {
                claims.Add(claim);
            }
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return new LocalSignInResult(RedirectToHome: true);
    }
}
