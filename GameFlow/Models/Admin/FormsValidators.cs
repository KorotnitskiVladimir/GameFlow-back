using GameFlow.Data;

namespace GameFlow.Models.Admin;

public class FormsValidators
{
    private readonly DataContext _dataContext;

    public FormsValidators(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public Dictionary<string, string> ValidateCategoryFormModel(CategoryFormModel? formModel)
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
    
    public Dictionary<string, string> ValidateProductFormModel(ProductFormModel? formModel)
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
    
    public Dictionary<string, string> ValidateActionFormModel(ActionFormModel? formModel)
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

    public Dictionary<string, string> ValidateProductAmendmentFormModel(ProductAmendmentFormModel? formModel)
    {
        Dictionary<string, string> errors = new();
        if (formModel == null)
        {
            errors["Model"] = "Data not received";
        }
        else
        {
            if (_dataContext.Products.FirstOrDefault(p => p.Name == formModel.Name) == null)
            {
                errors[nameof(formModel.Name)] = "Product not found";
            }
            
            if (formModel.TitleAction == "amend" && formModel.Title == null)
            {
                errors[nameof(formModel.Title)] = "Title image required";
            }

            if (formModel.HorizonAction == "add" || formModel.HorizonAction == "amend")
            {
                if (formModel.Horizon == null)
                {
                    errors[nameof(formModel.Horizon)] = "Horizon images required";
                }
            }
            
            if (formModel.VerticalAction == "add" || formModel.VerticalAction == "amend")
            {
                if (formModel.Vertical == null)
                {
                    errors[nameof(formModel.Vertical)] = "Vertical images required";
                }
            }
        }

        return errors;
    }
}