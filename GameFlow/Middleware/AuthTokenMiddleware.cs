using GameFlow.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace GameFlow.Middleware;

public class AuthTokenMiddleware
{
    private readonly RequestDelegate _next;

    public AuthTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, DataContext dataContext)
    {
        string authHeader = context.Request.Headers.Authorization.ToString();
        string? errorMessage = null;
        string scheme = "Bearer ";
        Guid jti = default;
        if (string.IsNullOrEmpty(authHeader))
        {
            errorMessage = "Authorization header required";
        }
        else if (!authHeader.StartsWith(scheme))
        {
            errorMessage = $"Authorization header must be {scheme}";
        }
        else
        {
            string credentials = authHeader[scheme.Length..];
            try
            {
                jti = Guid.Parse(credentials);
            }
            catch
            {
                errorMessage = "Authorization credentials invalid formatted";
            }
        }

        if (errorMessage == null)
        {
            var accessToken = dataContext.AccessTokens.Include(at => at.UserAccess)
                .ThenInclude(ua => ua.UserRole)
                .Include(at => at.User)
                .FirstOrDefault(at => at.Jti == jti);

            if (accessToken == null)
            {
                errorMessage = "Bearer credentials rejected";
            }
            else if (accessToken.Exp < DateTime.Now)
            {
                errorMessage = "Bearer credential expired";
            }
            else
            {
                var userAccess = accessToken.UserAccess;
                context.User = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        new Claim[]
                        {
                            new Claim(ClaimTypes.Sid, userAccess.Id.ToString()),
                            new Claim(ClaimTypes.Name, accessToken.User.UserName),
                            new Claim(ClaimTypes.Email, accessToken.User.Email),
                            new Claim(ClaimTypes.MobilePhone, accessToken.User.Phone),
                            new Claim(ClaimTypes.Actor, accessToken.User.Login),
                            new Claim(ClaimTypes.Country, accessToken.User.Country),
                            new Claim(ClaimTypes.Role, userAccess.UserRole.Id),
                            new Claim("CanCreate", userAccess.UserRole.CanCreate.ToString()),
                            new Claim("CanRead", userAccess.UserRole.CanRead.ToString()),
                            new Claim("CanUpdate", userAccess.UserRole.CanUpdate.ToString()),
                            new Claim("CanDelete", userAccess.UserRole.CanDelete.ToString())
                        },
                        nameof(AuthTokenMiddleware)));
                context.Items.Add("AccessToken", accessToken);
            }
        }
        context.Items.Add(nameof(AuthSessionMiddleware), errorMessage);
        await _next(context);
    }
}

public static class AuthTokenMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthToken(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthTokenMiddleware>();
    }
}