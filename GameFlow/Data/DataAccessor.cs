using System.ComponentModel;
using GameFlow.Services.KDF;
using GameFlow.Services.Storage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Linq;

namespace GameFlow.Data;

public class DataAccessor
{
    private readonly DataContext _dataContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IKDFService _kdfService;
    private readonly IstorageService _storageService;

    public DataAccessor(DataContext dataContext, IHttpContextAccessor httpContextAccessor, IKDFService kdfService,
        IstorageService storageService)
    {
        _dataContext = dataContext;
        _httpContextAccessor = httpContextAccessor;
        _kdfService = kdfService;
        _storageService = storageService;
    }

    private string ImagePath => 
        $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/Admin/Image/";

    public Product GetProduct(Product product)
    {
        product.ImagesCsv = string.Join(',', product.ImagesCsv.Split(',')
            .Select(i => ImagePath + i));
        
        if (!string.IsNullOrEmpty(product.HorisontalImages))
        {
            product.HorisontalImages = string.Join(',', product.HorisontalImages.Split(',')
                .Select(i => ImagePath + i));
        }

        if (!string.IsNullOrEmpty(product.VerticalImages))
        {
            product.VerticalImages = string.Join(',', product.VerticalImages.Split(',')
                .Select(i => ImagePath + i));
        }
        
        if (product.ActionId != null)
        {
            product.Action = _dataContext.Actions.FirstOrDefault(a => a.Id == product.ActionId);
        }
        
        return product;
    }
    
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
                Iss = "GameFlow"
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

    public List<Category> AllCategories()
    {
        var categories = _dataContext.Categories.Where(c => c.DeletedAt == null)
            .AsNoTracking()
            .ToList();
        foreach (var category in categories)
        {
            category.ImageUrl = ImagePath + category.ImageUrl;
        }

        return categories;
    }

    public Category? GetCategory(string slug)
    {
        var category = _dataContext.Categories
            .Include(c => c.Products)
            .AsNoTracking()
            .FirstOrDefault(c => c.Slug == slug);
        if (category != null)
        {
            category.ImageUrl = ImagePath + category.ImageUrl;
            foreach (var product in category!.Products)
            {
                GetProduct(product);
                //product.ImagesCsv = string.Join(',', product.ImagesCsv.Split(',')
                //    .Select(i => ImagePath + i));
                //if (product.ActionId != null)
                //{
                //    product.Action = _dataContext.Actions.FirstOrDefault(a => a.Id == product.ActionId);
                //}
            }
        }

        return category;
    }

    public string AmendUsersData(string data, UserData user)
    {
        string[] temp = data.Split(',', 4);
        string name = temp[0];
        string phone = temp[1];
        string country = temp[2];
        string about = temp[3];
        List<string> errors = new();
        if (!string.IsNullOrEmpty(name))
        {
            if (user.UserName != name)
            {
                user.UserName = name;
            }
        }
        else
        {
            errors.Add("Name can't be blank");
        }

        if (!string.IsNullOrEmpty(phone))
        {
            if (user.Phone != phone)
            {
                user.Phone = phone;
            }
        }
        else
        {
            errors.Add("Phone can't be blank");
        }

        if (!string.IsNullOrEmpty(country))
        {
            if (user.Country != country)
            {
                user.Country = country;
            }
        }
        else
        {
            errors.Add("Country can't be blank");
        }

        if (!string.IsNullOrEmpty(about))
        {
            if (user.AboutUser != about)
            {
                user.AboutUser = about;
            }
        }

        if (errors.Count == 0)
        {
            _dataContext.SaveChanges();
            return "Changes saved successfully";
        }
        else
        {
             return string.Join(',', errors);
        }
    }

    public UserData? GetUserData(Guid id)
    {
        var user = _dataContext.UsersData.FirstOrDefault(ud => ud.Id == id);
        if (user != null)
        {
            user.AvatarUrl = ImagePath + user.AvatarUrl;
            return user;
        }
        else
        {
            return null;
        }
    }

    public Product? GetProduct(string prodId)
    {
        Guid? productId = Guid.Parse(prodId);
        var product = _dataContext.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefault(p => p.Id == productId);
        if (product != null)
        {
            GetProduct(product);
        }

        return product;
    }

    public List<Product> AllProducts()
    {
        var products = _dataContext.Products.Where(p => p.DeletedAt == null)
            .AsNoTracking()
            .ToList();
        foreach (var product in products)
        {
            GetProduct(product);
        }

        return products;
    }

    public List<Product> GetTopRatedProducts()
    {
        var products = _dataContext.Products.OrderByDescending(p => p.Rating)
            .AsNoTracking()
            .ToList();
        foreach (var product in products)
        {
            GetProduct(product);
        }

        return products;
    }

    public List<Product> GetNewestProducts()
    {
        var products = _dataContext.Products.OrderByDescending(p => p.ReleaseDate)
            .AsNoTracking()
            .ToList();
        foreach (var product in products)
        {
            GetProduct(product);
        }

        return products;
    }
    
    public List<Product> GetCheapestProducts()
    {
        var products = _dataContext.Products.Where(p => p.Price < 100)
            .AsNoTracking()
            .ToList();
        foreach (var product in products)
        {
            GetProduct(product);
        }

        return products;
    }
}