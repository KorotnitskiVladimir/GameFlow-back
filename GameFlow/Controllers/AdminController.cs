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
    private readonly FormsValidators _formValidator;

    public AdminController(DataContext dataContext, IstorageService storageService, FormsValidators formValidator)
    {
        _dataContext = dataContext;
        _storageService = storageService;
        _formValidator = formValidator;
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
        Dictionary<string, string> errors = _formValidator.ValidateCategoryFormModel(formModel);
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

    public IActionResult Product()
    {
        ProductViewModel viewModel = new()
        {
            FormModel = new(),
            Categories = _dataContext.Categories.ToList()
        };
        
        return View(viewModel);
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

        Dictionary<string, string> errors = _formValidator.ValidateProductFormModel(formModel);
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
            
            if (!string.IsNullOrEmpty(formModel.Slug))
            {
                product.Slug = formModel.Slug;
            }
            
            if (formModel.HorizonImages != null)
            {
                product.HorisontalImages = string.Join(',', formModel.HorizonImages.Select(img => _storageService.SaveFile(img)));
            }

            if (formModel.VerticalImages != null)
            {
                product.VerticalImages = string.Join(',', formModel.VerticalImages.Select(img => _storageService.SaveFile(img)));
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
    
    [HttpPost]
    public JsonResult AddAction(ActionFormModel formModel)
    {

        Dictionary<string, string> errors = _formValidator.ValidateActionFormModel(formModel);
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

    public IActionResult ProductManagement()
    {
        ProductAmendmentViewModel viewModel = new()
        {
            FormModel = new()
        };

        return View(viewModel);
    }

    [HttpPost]
    public JsonResult AmendProduct(ProductAmendmentFormModel formModel)
    {
        Dictionary<string, string> errors = _formValidator.ValidateProductAmendmentFormModel(formModel);
        if (errors.Count == 0)
        {
            var product = _dataContext.Products.FirstOrDefault(p => p.Name == formModel.Name);
            if (formModel.TitleAction == "amend")
            {
                product.ImagesCsv = _storageService.SaveFile(formModel.Title);
            }

            if (formModel.HorizonAction != "nothing")
            {
                switch (formModel.HorizonAction)
                {
                    case "add":
                        if (product.HorisontalImages == null)
                        {
                            product.HorisontalImages +=
                                string.Join(',', formModel.Horizon.Select(img => _storageService.SaveFile(img)));
                        }
                        else
                        {
                            product.HorisontalImages += ',';
                            product.HorisontalImages +=
                                string.Join(',', formModel.Horizon.Select(img => _storageService.SaveFile(img)));
                        }
                        break;
                    case "delete":
                        product.HorisontalImages = null;
                        break;
                    case "amend":
                        product.HorisontalImages =
                            string.Join(',', formModel.Horizon.Select(img => _storageService.SaveFile(img)));
                        break;
                }
            }

            if (formModel.VerticalAction != "nothing")
            {
                switch (formModel.VerticalAction)
                {
                    case "add":
                        if (product.VerticalImages == null)
                        {
                            product.VerticalImages +=
                                string.Join(',', formModel.Vertical.Select(img => _storageService.SaveFile(img)));
                        }
                        else
                        {
                            product.VerticalImages += ',';
                            product.VerticalImages +=
                                string.Join(',', formModel.Vertical.Select(img => _storageService.SaveFile(img))); 
                        }
                        break;
                    case "delete":
                        product.VerticalImages = null;
                        break;
                    case "amend":
                        product.VerticalImages =
                            string.Join(',', formModel.Vertical.Select(img => _storageService.SaveFile(img)));
                        break;
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