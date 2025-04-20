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
}