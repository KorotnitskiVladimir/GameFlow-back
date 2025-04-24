using GameFlow.Data;
using GameFlow.Models.User;
using GameFlow.Services.KDF;
using GameFlow.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace GameFlow.Controllers;

public class UserController(DataContext dataContext, DataAccessor dataAccessor, IKDFService kdfService) : Controller
{
    private const String signupFormKey = "UserSignUpFormModel";
    private readonly DataContext _dataContext = dataContext;
    private readonly DataAccessor _dataAccessor = dataAccessor;
    private readonly IKDFService _kdfService = kdfService;
    // GET
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Signup()
    {
        UserSignUpViewModel viewModel = new();

        if (HttpContext.Session.Keys.Contains(signupFormKey))
        {
            viewModel.FormModel =
                JsonSerializer.Deserialize<UserSignUpFormModel>(
                    HttpContext.Session.GetString(signupFormKey)!
                );
            viewModel.ValidationErrors = ValidateSignUpFormModel(viewModel.FormModel);

            if (viewModel.ValidationErrors.Count == 0)
            {
                Guid userId = Guid.NewGuid();
                _dataContext.UsersData.Add(new()
                {
                    Id = userId,
                    UserName = viewModel.FormModel!.UserName,
                    Email = viewModel.FormModel!.UserEmail,
                    Phone = viewModel.FormModel.UserPhone,
                    Country = viewModel.FormModel.Country,
                    AvatarUrl = viewModel.FormModel.AvatarUrl,
                    AboutUser = viewModel.FormModel.AboutUser,
                    BirthDate = viewModel.FormModel!.BirthDate,
                    RegDate = DateTime.Now,
                });
                String salt = Guid.NewGuid().ToString()[..16];
                _dataContext.UserAccesses.Add(new()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Login = viewModel.FormModel!.UserLogin,
                    RoleId = "guest",
                    Salt = salt,
                    Dk = _kdfService.DerivedKey(
                        viewModel.FormModel!.UserPassword,
                        salt),
                });
                _dataContext.SaveChanges();
            }

            HttpContext.Session.Remove(signupFormKey);
        }
        return View(viewModel);
    }

    public IActionResult Signin()
    {
        AccessToken accessToken;
        try
        {
            accessToken = _dataAccessor.Authenticate(Request);
        }
        catch (Win32Exception ex)
        {
            return Json(new { status = ex.ErrorCode, message = ex.Message });
        }

        
        HttpContext.Session.SetString("userAccessId", accessToken.Sub.ToString());
        return Json(new { status = 200, message = "OK" });
    }



    public RedirectToActionResult Register([FromForm] UserSignUpFormModel formModel)
    {
        HttpContext.Session.SetString(            
            signupFormKey,                
            JsonSerializer.Serialize(formModel) 
        );
        return RedirectToAction(nameof(Signup));
    }
    private Dictionary<String, String> ValidateSignUpFormModel(UserSignUpFormModel? formModel)
    {
        Dictionary<String, String> errors = [];
        if (formModel == null)
        {
            errors["Model"] = "Дані не передані";
        }
        else
        {
            if (String.IsNullOrEmpty(formModel.UserName))
            {
                errors[nameof(formModel.UserName)] = "Ім'я необхідно ввести";
            }
            if (String.IsNullOrEmpty(formModel.UserEmail))
            {
                errors[nameof(formModel.UserEmail)] = "E-mail необхідно ввести";
            }
            if (String.IsNullOrEmpty(formModel.UserPhone))
            {
                errors[nameof(formModel.UserPhone)] = "Телефон необхідно ввести";
            }
            if (String.IsNullOrEmpty(formModel.UserLogin))
            {
                errors[nameof(formModel.UserLogin)] = "Логін необхідно ввести";
            }
            if (formModel.BirthDate == default)
            {
                errors[nameof(formModel.BirthDate)] = "Дата народження необхідно ввести";
            }
            if (String.IsNullOrEmpty(formModel.Country))
            {
                errors[nameof(formModel.Country)] = "Країну бажано ввести";
            }
            if (String.IsNullOrEmpty(formModel.AvatarUrl))
            {
                errors[nameof(formModel.AvatarUrl)] = "Тут повинно бути твоє найкраще фото";
            }
            if (String.IsNullOrEmpty(formModel.AboutUser))
            {
                errors[nameof(formModel.AboutUser)] = "Розкажи про себе";
            }
            else
            {
                _dataContext
                    .UserAccesses
                    .FirstOrDefault(ua => ua.Login == formModel.UserLogin != null);
            }

            if (String.IsNullOrEmpty(formModel.UserPassword))
            {
                errors[nameof(formModel.UserPassword)] = "Пароль необхідно ввести";
            }
        }
        return errors;

    }
}