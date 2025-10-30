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

    [HttpGet("{id}")]

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

    [HttpGet("product/{prodId}")]
    public RestResponse GetSelectedProduct(string prodId)
    {
        return new()
        {
            Service = "Api Product",
            DataType = "object",
            CacheTime = 600,
            Data = _dataAccessor.GetProduct(prodId)
        };
    }

    [HttpGet("allProducts")]
    public RestResponse GetAllProducts()
    {
        return new()
        {
            Service = "Api Products",
            DataType = "array",
            CacheTime = 600,
            Data = _dataAccessor.AllProducts()
        };
    }

    [HttpGet("topRatedProducts")]
    public RestResponse TopRatedProducts()
    {
        return new()
        {
            Service = "Api Products",
            DataType = "array",
            CacheTime = 600,
            Data = _dataAccessor.GetTopRatedProducts()
        };
    }

    [HttpGet("newestProducts")]
    public RestResponse NewestProducts()
    {
        return new()
        {
            Service = "Api product",
            DataType = "array",
            CacheTime = 600,
            Data = _dataAccessor.GetNewestProducts()
        };
    }
    
    [HttpGet("cheapestProducts")]
    public RestResponse CheapestProducts()
    {
        return new()
        {
            Service = "Api product",
            DataType = "array",
            CacheTime = 600,
            Data = _dataAccessor.GetCheapestProducts()
        };
    }
}