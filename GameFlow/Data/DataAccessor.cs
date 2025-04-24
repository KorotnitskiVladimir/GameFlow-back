using GameFlow.Services.KDF;
using GameFlow.Data.Entities;
using GameFlow.Migrations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;



namespace GameFlow.Data;

public class DataAccessor(DataContext dataContext, IHttpContextAccessor httpContextAccessor, IKDFService kdfService)
{
    private readonly DataContext _dataContext = dataContext;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IKDFService _kdfService = kdfService;

    private string ImagePath =>
        $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}/Shop/Image";

    public GameFlow.Data.Entities.AccessToken Authenticate(HttpRequest Request)
    {
        String authHeader = Request.Headers.Authorization.ToString();
        if (String.IsNullOrEmpty(authHeader))
        {
            throw new Win32Exception(401, "Authorization header required");
        }
        String scheme = "Basic ";
        if (!authHeader.StartsWith(scheme))
        {
            throw new Win32Exception(401, $"Authorization scheme must be {scheme}");
        }
        String credentials = authHeader[scheme.Length..];
        String authData;
        try
        {
            authData = System.Text.Encoding.UTF8.GetString(
                Base64UrlTextEncoder.Decode(credentials)
            );
        }
        catch
        {
            throw new Win32Exception(401, $"Not valid Base64 code '{credentials}'");
        }
        // authData == "login:password"  
        String[] parts = authData.Split(':', 2);
        if (parts.Length != 2)
        {
            throw new Win32Exception(401, $"Not valid credentials format (missing ':'?)");
        }
        String login = parts[0];
        String password = parts[1];
        var userAccess = _dataContext.UserAccesses.FirstOrDefault(ua => ua.Login == login);
        if (userAccess == null)
        {
            throw new Win32Exception(401, "Введіть логін!");
        }
        if (_kdfService.DerivedKey(password, userAccess.Salt) != userAccess.Dk && _kdfService.DerivedKey(password, userAccess.Salt) != null)
        {
            throw new Win32Exception(401, "Введіть пароль!");
        }

        GameFlow.Data.Entities.AccessToken accessToken = new()
        {
            Jti = Guid.NewGuid(),
            Sub = userAccess.Id,
            Aud = userAccess.UserId,
            Iat = DateTime.Now,
            Nbf = null,
            Exp = DateTime.Now.AddMinutes(10),
            Iss = "GameFlow"
        };
        _dataContext.AccessTokens.Add(accessToken);
        _dataContext.SaveChanges();
        return accessToken;
    }
}
