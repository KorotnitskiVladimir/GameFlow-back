using Microsoft.AspNetCore.Mvc;

namespace GameFlow.Controllers;

public class UserController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}