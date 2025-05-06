using GameFlow.Data;
using GameFlow.Models.Admin;
using GameFlow.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace GameFlow.Controllers;

/// <summary>
/// Через контроллер реализуем добавление категорий, товаров и акций в магазин
/// </summary>
public class AdminController : Controller
{
    private readonly DataContext _dataContext;
    private readonly IstorageService _storageService;

    public AdminController(DataContext dataContext, IstorageService storageService)
    {
        _dataContext = dataContext;
        _storageService = storageService;
    }
    
    public FileResult Image([FromRoute] string id)
    {
        return File(System.IO.File.ReadAllBytes(_storageService.GetRealPath(id)), "image/jpeg");
    }
    public IActionResult Index()
    {
        string? canCreate = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "CanCreate")?.Value;
        if (canCreate != "1")
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return NoContent();
        }
        
        return View();
    }

    public IActionResult Category()
    {
        CategoryFormViewModel viewModel = new()
        {
            FormModel = new()
        };

        return View(viewModel);
    }

    [HttpPost]
    public JsonResult AddCategory(CategoryFormModel formModel)
    {
        Dictionary<string, string> errors = ValidateCategoryFormModel(formModel);
        if (errors.Count == 0)
        {
            Category category = new()
            {
                Id = Guid.NewGuid(),
                ParentId = null,
                Name = formModel.Name,
                Description = formModel.Description,
                Slug = formModel.Slug,
                ImageUrl = _storageService.SaveFile(formModel.Image)
            };
            var parent = _dataContext.Categories
                .FirstOrDefault(c => c.Slug == formModel.ParentCategory);
            if (parent != null)
            {
                category.ParentId = parent.Id;
                category.ParentCategory = parent;
            }

            _dataContext.Categories.Add(category);
            _dataContext.SaveChanges();
            return Json(formModel);
        }
        else
        {
            return Json(new { status = 401, message = errors.Values });
        }
    }

    private Dictionary<string, string> ValidateCategoryFormModel(CategoryFormModel? formModel)
    {
        Dictionary<string, string> errors = new();
        if (formModel == null)
        {
            errors["Model"] = "Data not received";
        }
        else
        {
            if (string.IsNullOrEmpty(formModel.Name))
            {
                errors[nameof(formModel.Name)] = "Name required";
            }

            if (!string.IsNullOrEmpty(formModel.ParentCategory))
            {
                if (_dataContext.Categories.FirstOrDefault(c => c.Slug == formModel.ParentCategory) == null)
                {
                    errors[nameof(formModel.ParentCategory)] = "Such parent category not found";
                }
            }

            if (string.IsNullOrEmpty(formModel.Description))
            {
                errors[nameof(formModel.Description)] = "Description required";
            }

            if (string.IsNullOrEmpty(formModel.Slug))
            {
                errors[nameof(formModel.Slug)] = "Slug required";
            }
            else
            {
                if (_dataContext.Categories.FirstOrDefault(c => c.Slug == formModel.Slug) != null)
                {
                    errors[nameof(formModel.Slug)] = "Such category exists already";
                }
            }

            if (string.IsNullOrEmpty(formModel.Image.FileName))
            {
                errors[nameof(formModel.Image)] = "Image required";
            }
        }

        return errors;
    }
}