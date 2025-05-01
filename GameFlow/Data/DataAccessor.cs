using System.ComponentModel;
using GameFlow.Services.KDF;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace GameFlow.Data;

public class DataAccessor
{
    private readonly DataContext _dataContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IKDFService _kdfService;

    public DataAccessor(DataContext dataContext, IHttpContextAccessor httpContextAccessor, IKDFService kdfService)
    {
        _dataContext = dataContext;
        _httpContextAccessor = httpContextAccessor;
        _kdfService = kdfService;
    }

    private string ImagePath => 
        $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/Shop/Image";
    
    public AccessToken Authenticate(HttpRequest Request)
    {
        string authHeader = Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader))
        {
            throw new Win32Exception(401, "Authorization header required");
        }

        string scheme = "Basic ";
        if (!authHeader.StartsWith(scheme))
        {
            throw new Win32Exception(401, $"Authorization scheme must be {scheme}");
        }

        string credentials = authHeader[scheme.Length..];
        string authData;
        try
        {
            authData = System.Text.Encoding.UTF8.GetString(Base64UrlTextEncoder.Decode(credentials));
        }
        catch
        {
            throw new Win32Exception(401, $"Not valid Base64 code '{credentials}'");
        }
        // authData == "login:password"
        string[] parts = authData.Split(':', 2);
        if (parts.Length != 2)
        {
            throw new Win32Exception(401, "Not valid credentials format (missing ':'?)");
        }

        string login = parts[0];
        string password = parts[1];
        var userAccess = _dataContext.UserAccesses.FirstOrDefault(ua => ua.Login == login);
        if (userAccess == null)
        {
            throw new Win32Exception(401, "Credentials rejected");
        }
        
        if (_kdfService.DerivedKey(password, userAccess.Salt) != userAccess.Dk)
        {
            throw new Win32Exception(401, "Credentials rejected.");
        }
        
        //Серж, тут тоже нужно было сначала добавить проверку существует ли уже такой токен
        var sub = _dataContext.AccessTokens.FirstOrDefault(at => at.Sub == userAccess.Id);
        if (sub != null)
        {
            sub.Exp = DateTime.Now.AddMinutes(10);
            _dataContext.SaveChanges();
            return sub;
        }
        else
        {
            AccessToken accessToken = new()
            {
                Jti = Guid.NewGuid(),
                Sub = userAccess.Id,
                Aud = userAccess.UserId,
                Iat = DateTime.Now,
                Nbf = null,
                Exp = DateTime.Now.AddMinutes(10),
                Iss = "ASP"
            };
            _dataContext.AccessTokens.Add(accessToken);
            _dataContext.SaveChanges();
            return accessToken;
        }
    }

    public AccessToken Authorize(HttpRequest request)
    {
        string authHeader = request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader))
        {
            throw new Win32Exception(401, "Authorization header required");
        }

        string scheme = "Bearer ";
        if (!authHeader.StartsWith(scheme))
        {
            throw new Win32Exception(401, $"Authorization scheme must be {scheme}");
        }

        string credentials = authHeader[scheme.Length..];
        Guid jti;
        try
        {
            jti = Guid.Parse(credentials);
        }
        catch
        {
            throw new Win32Exception(401, "Authorization credentials invalid formatted");
        }

        AccessToken? accessToken = _dataContext.AccessTokens.Include(at => at.User)
            .FirstOrDefault(at => at.Jti == jti);

        if (accessToken == null)
        {
            throw new Win32Exception(401, "Bearer credentials rejected");
        }

        if (accessToken.Exp < DateTime.Now)
        {
            throw new Win32Exception(401, "Bearer credentials expired");
        }

        return accessToken;
    }
}