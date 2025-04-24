using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using GameFlow.Data;

namespace GameFlow.Middleware;

public class AuthSessionMiddleware
{
    private readonly RequestDelegate _next;

    public AuthSessionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, DataContext dataContext)
    {
        if (context.Request.Query.ContainsKey("logout"))
        {
            context.Session.Remove("userAccessId");
            context.Response.Redirect(context.Request.Path);
            return;
        }

        if (context.Session.Keys.Contains("userAccessId"))
        {
            // пользователь аутентифицирован
            context.Items.Add("auth", "OK");

            if (dataContext.UserAccesses
                    .Include(ua => ua.UserData)
                    .Include(ua => ua.UserRole)
                    .FirstOrDefault(ua => ua.Id.ToString() == context.Session.GetString("userAccessId"))
                is UserAccess userAccess)
            {
                context.User = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        new Claim[]
                        {
                            new Claim(ClaimTypes.Sid, userAccess.Id.ToString()),
                            new Claim(ClaimTypes.Name, userAccess.UserData.UserName),
                            new Claim(ClaimTypes.Email, userAccess.UserData.Email),
                            new Claim(ClaimTypes.MobilePhone, userAccess.UserData.Phone),
                            new Claim(ClaimTypes.Actor, userAccess.UserData.Login),
                            new Claim(ClaimTypes.Country, userAccess.UserData.Country),
                            new Claim(ClaimTypes.Role, userAccess.UserRole.Id),
                            new Claim("CanCreate", userAccess.UserRole.CanCreate.ToString()),
                            new Claim("CanRead", userAccess.UserRole.CanRead.ToString()),
                            new Claim("CanUpdate", userAccess.UserRole.CanUpdate.ToString()),
                            new Claim("CanDelete", userAccess.UserRole.CanDelete.ToString())
                        }, nameof(AuthSessionMiddleware))); 
            }
        }
        await _next(context);
    }
}

public static class AuthSessionMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthSession(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthSessionMiddleware>();
    }
}