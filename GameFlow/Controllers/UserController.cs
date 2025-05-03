using System.ComponentModel;
using System.Text.Json;
using GameFlow.Data;
using GameFlow.Models.User;
using GameFlow.Services.KDF;
using GameFlow.Services.Salt;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Linq;

namespace GameFlow.Controllers;

public class UserController : Controller
{
    // GET
    private readonly DataContext _dataContext;
    private readonly IKDFService _kdfService;
    private readonly ISaltGeneratorService _saltGenerator;
    private readonly DataAccessor _dataAccessor;

    public UserController(DataContext dataContext, IKDFService kdfService, ISaltGeneratorService saltGenerator, DataAccessor dataAccessor)
    {
        _dataContext = dataContext;
        _kdfService = kdfService;
        _saltGenerator = saltGenerator;
        _dataAccessor = dataAccessor;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult SignUp()
    {
        UserSignUpViewModel viewModel = new()
        {
            FormModel = new()
        };

        return View(viewModel);
    }

    [HttpPost]
    public JsonResult Register(UserSignUpFormModel? formModel)
    {
        // Серж, сразу комментарии по твоему контроллеру:
        // 1. аватар и о себе - это необязательные поля. на этапе регистрации пользователя мы не делаем проверку на пустое поле
        // 2. проверка существующего логина выполняется сразу за блоком проверки поля на "пустоту". + у тебя ошибку не
        // возвращает, елси поользователь с таким логином уже существем в базе
        // 3. нет проверки на совпадение паролей
        if (formModel == null)
        {
            return Json(new { status = 401, message = "data not received" });
        }

        Dictionary<string, string> errors = ValidateUserSingUpFormModel(formModel);
        if (errors.Count == 0)
        {
            Guid userId = Guid.NewGuid();
            UserData userData = new()
            {
                Id = userId,
                Login = formModel.UserLogin,
                UserName = formModel.UserName,
                Email = formModel.UserEmail,
                Phone = formModel.UserPhone,
                Country = formModel.Country,
                BirthDate = formModel.BirthDate,
                RegDate = DateTime.Now
            };
            _dataContext.UsersData.Add(userData);
            string salt = _saltGenerator.GenerateSalt(16);
            _dataContext.UserAccesses.Add(new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Login = formModel.UserLogin,
                RoleId = "guest", // первым регистрируем админа. потом меняем на гостей. по желанию можно добавить модераторов и/или эдиторов
                Salt = salt,
                Dk = _kdfService.DerivedKey(formModel.UserPassword, salt)
            });
            _dataContext.SaveChanges();
            return Json(formModel);
        }
        else
        {
            return Json(new { status = 401, message = errors.Values });
        }
    }

    private Dictionary<string, string> ValidateUserSingUpFormModel(UserSignUpFormModel? formModel)
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

            if ((DateTime.Now.Year - formModel.BirthDate.Year) < 18)
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

    public IActionResult Signin()
    {
        AccessToken accessToken;
        try
        {
            accessToken = _dataAccessor.Authenticate(Request);
        }
        catch (Win32Exception e)
        {
            return Json(new { status = e.ErrorCode, message = e.Message });
        }
        HttpContext.Session.SetString("userAccessId", accessToken.Sub.ToString());
        return Json(new { status = 200, message = "OK" });
    }

    public ViewResult UserReview()
    {
        UserSignUpViewModel viewModel = new()
        {
            FormModel = new()
        };

        return View(viewModel);
    }
    public ViewResult UserAmendment()
    {
        UserSignUpViewModel viewModel = new()
        {
            FormModel = new()
        };

        return View(viewModel);
    }

    [HttpPut]
    public JsonResult Change([FromQuery] string login, [FromQuery] string name, [FromQuery] string phone, 
                             [FromQuery] string email, [FromQuery] string country, [FromQuery] string avatar,
                             [FromQuery] string aboutuser, [FromQuery] string uaId)
    {
        UserAccess? userAccess = _dataContext.UserAccesses
            .FirstOrDefault(u => u.Id.ToString() == uaId);

        if (userAccess != null)
        {
            UserData? user = _dataContext.UsersData
                .FirstOrDefault(u => u.Id == userAccess.UserId);

            if (user != null)
            {
                user.Login = login;
                user.UserName = name;
                user.Phone = phone;
                user.Email = email;
                user.Country = country;
                user.AvatarUrl = avatar;
                user.AboutUser = aboutuser;
            }
            if (login != null)
            {
                userAccess.Login = login;
            }
        }
        else
        {
            return Json(new { status = 404, message = "User access not found" });
        }
        _dataContext.SaveChanges();
        return Json(new { status = 200, message = "Modified" });
    }
}
