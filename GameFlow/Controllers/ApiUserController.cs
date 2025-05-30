﻿using System.ComponentModel;
using System.Text;
using GameFlow.Data;
using GameFlow.Models;
using GameFlow.Models.User;
using GameFlow.Services.KDF;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using GameFlow.Middleware;
using GameFlow.Services.Date;
using GameFlow.Services.Salt;
using GameFlow.Services.Storage;
using Microsoft.AspNetCore.Authentication;
using Syncfusion.EJ2.PivotView;

namespace GameFlow.Controllers;

[Route("api/user")]
[ApiController]
public class ApiUserController : ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly IKDFService _kdfService;
    private readonly ISaltGeneratorService _saltGenerator;
    private readonly IAgeCalculatorService _ageCalculator;
    private readonly DataAccessor _dataAccessor;
    private readonly IstorageService _storageService;

    public ApiUserController(DataContext dataContext, IKDFService kdfService, ISaltGeneratorService saltGenerator, 
        IAgeCalculatorService ageCalculator, DataAccessor dataAccessor, IstorageService storageService)
    {
        _dataContext = dataContext;
        _kdfService = kdfService;
        _saltGenerator = saltGenerator;
        _ageCalculator = ageCalculator;
        _dataAccessor = dataAccessor;
        _storageService = storageService;
    }
    
    [HttpPost]
    public RestResponse Register(UserApiSignUpFormModel? formModel)
    {
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
                IsOk = false,
                Phrase = "Data not received"
            };
            res.Data = null;
        }
        res.Data = formModel;
        Dictionary<string, string> errors = ValidateUserSingUpFormModel(formModel);
        if (errors.Count == 0)
        {
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
            string salt = _saltGenerator.GenerateSalt(16);
            _dataContext.UserAccesses.Add(new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Login = formModel!.UserLogin,
                RoleId = "guest",
                Salt = salt,
                Dk = _kdfService.DerivedKey(formModel!.UserPassword, salt)
            });
            _dataContext.SaveChanges();
        }
        else
        {
            res.Status = new()
            {
                IsOk = false,
                Phrase = string.Join(',', errors.Values)
            };
            res.Data = null;
        }

        return res;
    }
    
    private Dictionary<string, string> ValidateUserSingUpFormModel(UserApiSignUpFormModel? formModel)
    {
        Dictionary<string, string> errors = new();
        if (formModel != null)
        {
            if (string.IsNullOrEmpty(formModel.UserLogin))
            {
                errors[nameof(formModel.UserLogin)] = "Login required";
            }
            
            if (!string.IsNullOrEmpty(formModel.UserLogin))
            {
                if (_dataContext.UsersData.FirstOrDefault(u => u.Login == formModel.UserLogin) != null)
                {
                    errors[nameof(formModel.UserLogin)] =
                        "User with such login exists already. Please choose another one";
                }
            }

            if (string.IsNullOrEmpty(formModel.UserName))
            {
                errors[nameof(formModel.UserName)] = "Name is required";
            }

            if (string.IsNullOrEmpty(formModel.UserPassword))
            {
                errors[nameof(formModel.UserPassword)] = "Password required";
            }

            if (string.IsNullOrEmpty(formModel.PasswordRepeat))
            {
                errors[nameof(formModel.PasswordRepeat)] = "Password repeat required";
            }

            if (!string.IsNullOrEmpty(formModel.UserPassword))
            {
                if (formModel.UserPassword != formModel.PasswordRepeat)
                {
                    errors[nameof(formModel.PasswordRepeat)] = "Passwords do not match each other";
                }
            }

            if (string.IsNullOrEmpty(formModel.UserPhone))
            {
                errors[nameof(formModel.UserPhone)] = "Phone required";
            }

            if (string.IsNullOrEmpty(formModel.UserEmail))
            {
                errors[nameof(formModel.UserEmail)] = "Email required";
            }

            if (string.IsNullOrEmpty(formModel.Country))
            {
                errors[nameof(formModel.Country)] = "Country required";
            }

            if (formModel.BirthDate == DateTime.MinValue)
            {
                errors[nameof(formModel.BirthDate)] = "Birth date required";
            }

            if (_ageCalculator.CalculateAge(formModel.BirthDate) < 18)
            {
                errors[nameof(formModel.BirthDate)] = "You should be at least 18 years old to register";
            }
        }
        else
        {
            errors["model"] = "Data not transferred";
        }

        return errors;
    }

    [HttpGet]
    public RestResponse Authenticate()
    {
        var res = new RestResponse()
        {
            Service = "Api User Authentication",
            DataType = "object",
            CacheTime = 600,
        };
        try
        {
            res.Data = _dataAccessor.Authenticate(Request);
        }
        catch (Win32Exception ex)
        {
            res.Status = new()
            {
                IsOk = false,
                Code = ex.HResult,
                Phrase = ex.Message
            };
            res.Data = null;
        }

        return res;
    }

    
    [HttpGet("jwt")]
    public RestResponse AuthenticateJwt()
    {
        var res = new RestResponse()
        {
            Service = "Api User Authentication",
            DataType = "object",
            CacheTime = 600,
        };
        try
        {
            string header = Base64UrlTextEncoder.Encode(
                Encoding.UTF8.GetBytes("{  \"alg\": \"HS256\",  \"typ\": \"JWT\"}"));
            string payload = Base64UrlTextEncoder.Encode(
                Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(_dataAccessor.Authenticate(Request))));
            string data = header + "." + payload;
            string signature = Base64UrlTextEncoder.Encode(System.Security.Cryptography.HMACSHA256.HashData(
                Encoding.UTF8.GetBytes("secret"),
                Encoding.UTF8.GetBytes(data)));
            res.Data = data + "." + signature;
        }
        catch (Win32Exception e)
        {
            res.Status = new()
            {
                IsOk = false,
                Code = e.HResult,
                Phrase = e.Message
            };
            res.Data = null;
        }

        return res;
    }
    
    
    [HttpGet("profile")]
    public RestResponse Profile()
    {
        var res = new RestResponse()
        {
            Service = "Api User Profile",
            DataType = "object",
            CacheTime = 600
        };
        if (HttpContext.User.Identity?.IsAuthenticated ?? false)
        {
            var user = (HttpContext.Items["AccessToken"] as AccessToken)?.User;
            //res.Data = (HttpContext.Items["AccessToken"] as AccessToken)?.User;
            //user.AvatarUrl = _dataAccessor.GetImagePath() + user.AvatarUrl;
            if (user != null)
                res.Data = _dataAccessor.GetUserData(user.Id);
            else
            {
                res.Data = null;
                res.Status = new()
                {
                    IsOk = false,
                    Code = 401,
                    Phrase = "User Not Found"
                };
            }
        }
        else
        {
            res.Status = new()
            {
                IsOk = false,
                Code = 401,
                Phrase = HttpContext.Items[nameof(AuthTokenMiddleware)]?.ToString() ?? ""
            };
            res.Data = null;
        }
        return res;
    }

    [HttpGet("{input}")]
    public RestResponse UserAmendment(string? input)
    {
        var res = new RestResponse()
        {
            Service = "Api User Amendment",
            DataType = "object",
            CacheTime = 600
        };
        if (HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var user = _dataContext.UsersData
                .FirstOrDefault(ud => ud.Id ==
                                      (HttpContext.Items["AccessToken"] as AccessToken).Aud);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(input))
                {
                    res.Data = _dataAccessor.AmendUsersData(input, user);
                }
                else
                {
                    res.Status = new()
                    {
                        IsOk = false,
                        Code = 401,
                        Phrase = "Data not accepted"
                    };
                    res.Data = null;
                }
            }
            else
            {
                res.Status = new()
                {
                    IsOk = false,
                    Code = 401,
                    Phrase = "User not found"
                };
                res.Data = null;
            }
        }

        return res;
    }

    [HttpPost("setAvatar")]
    public RestResponse SetAvatar([FromForm]IFormFile? formFile)
    {
        var res = new RestResponse()
        {
            Service = "Api User Avatar",
            DataType = "object",
            CacheTime = 600,
        };
        if (formFile != null)
        {
            if (HttpContext.User.Identity?.IsAuthenticated == true)
            {
                var user = _dataContext.UsersData
                    .FirstOrDefault(ud => ud.Id ==
                                          (HttpContext.Items["AccessToken"] as AccessToken).Aud);
                if (user != null)
                {
                    var avatarUrl = _storageService.SaveFile(formFile);
                    user.AvatarUrl = avatarUrl;
                    _dataContext.SaveChanges();
                    res.Data = avatarUrl;
                }
                else
                {
                    res.Status = new()
                    {
                        IsOk = false,
                        Code = 401,
                        Phrase = "User not found"
                    };
                    res.Data = null;
                }
            }
        }
        else
        {
            res.Status = new()
            {
                IsOk = false,
                Code = 401,
                Phrase = "Data not received"
            };
            res.Data = null;
        }

        return res;
    }
}