using GameFlow.Data;
using GameFlow.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameFlow.Controllers;

[Route("api/shop")]
[ApiController]
public class ApiShopController : ControllerBase
{
    private readonly DataAccessor _dataAccessor;

    public ApiShopController(DataAccessor dataAccessor)
    {
        _dataAccessor = dataAccessor;
    }
    
    [HttpGet("allCategories")]
    public RestResponse GetAllCategories()
    {
        return new()
        {
            Service = "Api Categories",
            DataType = "array",
            CacheTime = 600,
            Data = _dataAccessor.AllCategories()
        };
    }

    [HttpGet("categoryById {id}")]
    public RestResponse GetSelectedCategory(string id)
    {
        return new()
        {
            Service = "Api Categories",
            DataType = "object",
            CacheTime = 600,
            Data = _dataAccessor.GetCategory(id)
        };
    }
}