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
    public RestResponse Categories()
    {
        return new()
        {
            Service = "Api Categories",
            DataType = "array",
            CacheTime = 600,
            Data = _dataAccessor.AllCategories()
        };
    }
}