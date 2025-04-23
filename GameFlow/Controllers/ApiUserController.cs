using System.ComponentModel;
using GameFlow.Data;
using GameFlow.Models;
using GameFlow.Models.User;
using GameFlow.Services.KDF;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GameFlow.Controllers;

[Route("api/user")]
[ApiController]
public class ApiUserController : ControllerBase
{
    private const string signupFormKey = "UserSignupFormModel";
    private readonly DataContext _dataContext;
    private readonly IKDFService _kdfService;

    public ApiUserController(DataContext dataContext, IKDFService kdfService)
    {
        _dataContext = dataContext;
        _kdfService = kdfService;
    }

    [HttpOptions]
    
    [HttpPost]
    public RestResponse Register([FromBody] UserSignUpFormModel? formModel)
    {
        //var formModel = JsonSerializer.Deserialize<UserSignUpFormModel>(model);
        //var formModel = JsonSerializer.Deserialize<UserSignUpFormModel>(HttpContext.Session.GetString(signupFormKey));
        var res = new RestResponse()
        {
            Service = "Api User Registration",
            DataType = "object",
            CacheTime = 600,
        };
        if (formModel == null)
        {
            res.Status = new()
            {
                IsOk = false
            };
            res.Data = null;
        }

        res.Data = formModel;
        Guid userId = Guid.NewGuid();
        _dataContext.UsersData.Add(new()
        {
            Id = userId,
            Login = formModel!.UserLogin,
            UserName = formModel!.UserName,
            Email = formModel!.UserEmail,
            Country = formModel!.Country,
            Phone = formModel!.UserPhone,
            AvatarUrl = formModel?.AvatarUrl,
            AboutUser = formModel?.AboutUser,
            BirthDate = formModel!.BirthDate,
            RegDate = DateTime.Now
        });
        string salt = "salt";
        _dataContext.UserAccesses.Add(new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Login = formModel!.UserLogin,
            RoleId = "admin",
            Salt = salt,
            Dk = _kdfService.DerivedKey(formModel!.UserPassword, salt)
        });
        _dataContext.SaveChanges();
        HttpContext.Session.Remove(signupFormKey);
        return res;
    }
    
}