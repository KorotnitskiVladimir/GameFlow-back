using GameFlow.Data;
using GameFlow.Models.Admin;
using GameFlow.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using GameFlow.Services.Date;
using Action = GameFlow.Data.Action;

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

    public IActionResult Product()
    {
        ProductViewModel viewModel = new()
        {
            FormModel = new(),
            Categories = _dataContext.Categories.ToList()
        };
        
        return View(viewModel);
    }

    private Dictionary<string, string> ValidateProductFormModel(ProductFormModel? formModel)
    {
        double price;
        try
        {
            price = double.Parse(formModel.Price, System.Globalization.CultureInfo.InvariantCulture);
        }
        catch
        {
            price = double.Parse(formModel.Price.Replace(',', '.'),
                System.Globalization.CultureInfo.InvariantCulture);
        }
        Dictionary<string, string> errors = new();
        if (formModel == null)
        {
            errors["Model"] = "Data not received";
        }
        else
        {
            if (_dataContext.Categories.FirstOrDefault(c => c.Id == formModel.CategoryId) == null)
            {
                errors[nameof(formModel.CategoryId)] = "Category not found";
            }
            if (string.IsNullOrEmpty(formModel.Name))
            {
                errors[nameof(formModel.Name)] = "Name required";
            }

            if (string.IsNullOrEmpty(formModel.Description))
            {
                errors[nameof(formModel.Description)] = "Description required";
            }

            if (!string.IsNullOrEmpty(formModel.Slug))
            {
                if (_dataContext.Categories.FirstOrDefault(c => c.Slug == formModel.Slug) != null)
                {
                    errors[nameof(formModel.Slug)] = "Such product exists already";
                }
            }
            
            if (string.IsNullOrEmpty(formModel.Images.ToString()))
            {
                errors[nameof(formModel.Images)] = "Image(s) required";
            }

            if (formModel.Rating <= 0 || formModel.Rating > 100)
            {
                errors[nameof(formModel.Rating)] = "Rating is out of allowed range";
            }

            if (price <= 0)
            {
                errors[nameof(formModel.Price)] = "Price can't be less or equal to zero";
            }

            if (string.IsNullOrEmpty(formModel.Developer))
            {
                errors[nameof(formModel.Developer)] = "Developer required";
            }

            if (string.IsNullOrEmpty(formModel.Publisher))
            {
                errors[nameof(formModel.Publisher)] = "Publisher required";
            }

            if (string.IsNullOrEmpty(formModel.Tags))
            {
                errors[nameof(formModel.Tags)] = "Tags required";
            }

            if (string.IsNullOrEmpty(formModel.SupportedMods))
            {
                errors[nameof(formModel.SupportedMods)] = "Supported mods required";
            }

            if (string.IsNullOrEmpty(formModel.SupportedPlatforms))
            {
                errors[nameof(formModel.SupportedPlatforms)] = "Supported platforms required";
            }

            if (formModel.ReleaseDate == default)
            {
                errors[nameof(formModel.ReleaseDate)] = "Release date required";
            }
        }

        return errors;
    }

    [HttpPost]
    public JsonResult AddProduct(ProductFormModel formModel)
    {
        double price;
        try
        {
            price = double.Parse(formModel.Price, System.Globalization.CultureInfo.InvariantCulture);
        }
        catch
        {
            price = double.Parse(formModel.Price.Replace(',', '.'),
                System.Globalization.CultureInfo.InvariantCulture);
        }

        Dictionary<string, string> errors = ValidateProductFormModel(formModel);
        if (errors.Count == 0)
        {
            Data.Product product = new()
            {
                Id = Guid.NewGuid(),
                CategoryId = formModel.CategoryId,
                Rating = formModel.Rating,
                Price = price,
                Name = formModel.Name,
                Description = formModel.Description,
                Slug = formModel.Slug,
                ImagesCsv = string.Join(',', formModel.Images.Select(img => _storageService.SaveFile(img))),
                Developer = formModel.Developer,
                Publisher = formModel.Publisher,
                Tags = new(),
                SupportedMods = new(),
                SupportedPlatforms = new(),
                ReleaseDate = formModel.ReleaseDate
            };
            //string[] tags = formModel.Tags.Split(',');
            //string[] mods = formModel.SupportedMods.Split(',');
            //string[] platforms = formModel.SupportedPlatforms.Split(',');
            foreach (var tag in formModel.Tags.Split(','))
            {
                product.Tags.Add(tag);
            }
            foreach (var mod in formModel.SupportedMods.Split(','))
            {
                product.SupportedMods.Add(mod);
            }
            foreach (var platform in formModel.SupportedPlatforms.Split(','))
            {
                product.SupportedPlatforms.Add(platform);
            }

            _dataContext.Products.Add(product);
            _dataContext.SaveChanges();
            return Json(formModel);
        }
        else
        {
            return Json(new { status = 401, message = errors.Values });
        }
    }
    
    public IActionResult Action()
    {
        ActionViewModel? viewModel = new()
        {
            FormModel = new()
        };
        return View(viewModel);
    }
    
    private Dictionary<string, string> ValidateActionFormModel(ActionFormModel? formModel)
    {
        Dictionary<string, string> errors = new();
        if (formModel == null)
        {
            errors["Model"] = "Data not received";
        }
        else
        {
            if (string.IsNullOrEmpty(formModel.ApplicantType))
            {
                errors[nameof(formModel.ApplicantType)] = "Applicant type required";
            }
            
            if (string.IsNullOrEmpty(formModel.ApplicantName))
            {
                errors[nameof(formModel.ApplicantName)] = "Applicant name required";
            }
            
            if (!string.IsNullOrEmpty(formModel.ApplicantType) && !string.IsNullOrEmpty(formModel.ApplicantName))
            {
                if (formModel.ApplicantType == "product")
                {
                    var products = _dataContext.Products.Select(p => p.Name);
                    if (!products.Contains(formModel.ApplicantName))
                    {
                        errors[nameof(formModel.ApplicantName)] = "Such product not found";
                    }
                }
                
                if (formModel.ApplicantType == "category")
                {
                    var categories = _dataContext.Categories.Select(c => c.Name);
                    if (!categories.Contains(formModel.ApplicantName))
                    {
                        errors[nameof(formModel.Name)] = "Such category not found";
                    }
                }

                if (formModel.ApplicantType == "developer")
                {
                    var developers = _dataContext.Products.Select(p => p.Developer);
                    if (!developers.Contains(formModel.ApplicantName))
                    {
                        errors[nameof(formModel.ApplicantName)] = "Such developer not found";
                    }
                }

                if (formModel.ApplicantType == "publisher")
                {
                    var publishers = _dataContext.Products.Select(p => p.Publisher);
                    if (!publishers.Contains(formModel.ApplicantName))
                    {
                        errors[nameof(formModel.ApplicantName)] = "Such publisher not found";
                    }
                }
            }
            
            if (string.IsNullOrEmpty(formModel.Name))
            {
                errors[nameof(formModel.Name)] = "Name required";
            }
            
            if (string.IsNullOrEmpty(formModel.Description))
            {
                errors[nameof(formModel.Description)] = "Description required";
            }

            if (formModel.Amount <= 0 || formModel.Amount >= 100)
            {
                errors[nameof(formModel.Amount)] = "Amount is out of allowed range";
            }

            if (formModel.StartDate == default)
            {
                errors[nameof(formModel.StartDate)] = "Start date required";
            }
            
            if (formModel.EndDate == default)
            {
                errors[nameof(formModel.StartDate)] = "Start date required";
            }
            
            if (formModel.EndDate <= DateTime.Now)
            {
                errors[nameof(formModel.StartDate)] = "End date can't be less than current date";
            }
        }

        return errors;
    }
    
    [HttpPost]
    public JsonResult AddAction(ActionFormModel formModel)
    {

        Dictionary<string, string> errors = ValidateActionFormModel(formModel);
        if (errors.Count == 0)
        {
            Action action = new()
            {
                Id = Guid.NewGuid(),
                Name = formModel.Name,
                Description = formModel.Description,
                Amount = formModel.Amount,
                StartDate = formModel.StartDate,
                EndDate = formModel.EndDate
            };

            _dataContext.Actions.Add(action);
            if (formModel.ApplicantType == "product")
            {
                var product = _dataContext.Products.FirstOrDefault(p => p.Name == formModel.ApplicantName);
                product.ActionId = action.Id;
            }
            else if (formModel.ApplicantType == "category")
            {
                var category = _dataContext.Categories.FirstOrDefault(c => c.Name == formModel.ApplicantName);
                var products = _dataContext.Products.Where(p => p.CategoryId == category.Id);
                foreach (var product in products)
                {
                    product.ActionId = action.Id;
                }
            }
            else if (formModel.ApplicantType == "developer")
            {
                var products = _dataContext.Products.Where(p => p.Developer == formModel.ApplicantName);
                foreach (var product in products)
                {
                    product.ActionId = action.Id;
                }
            }
            else if (formModel.ApplicantType == "publisher")
            {
                var products = _dataContext.Products.Where(p => p.Publisher == formModel.ApplicantName);
                foreach (var product in products)
                {
                    product.ActionId = action.Id;
                }
            }
            _dataContext.SaveChanges();
            return Json(formModel);
        }
        else
        {
            return Json(new { status = 401, message = errors.Values });
        }
    }
}