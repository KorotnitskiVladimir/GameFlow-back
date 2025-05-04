using Microsoft.AspNetCore.Mvc;

namespace GameFlow.Models.Admin;

public class CategoryFormModel
{
    [FromForm(Name = "category-name")]
    public string Name { get; set; } = null!;
    [FromForm(Name = "parent-category")]
    public string? ParentCategory { get; set; }
    [FromForm(Name = "category-description")]
    public string Description { get; set; } = null!;
    [FromForm(Name = "category-slug")]
    public string Slug { get; set; } = null!;
    [FromForm(Name = "category-image")]
    public IFormFile Image { get; set; } = null!;
    
}