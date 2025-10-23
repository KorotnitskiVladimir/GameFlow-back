using Microsoft.AspNetCore.Mvc;

namespace GameFlow.Models.Admin;

public class ProductAmendmentFormModel
{
    [FromForm(Name = "product-name")] public string Name { get; set; } = null!;
    
    [FromForm(Name = "title")] public string TitleAction { get; set; } = null!;
    
    [FromForm(Name = "product-image")] public IFormFile? Title { get; set; }

    [FromForm(Name = "horizon")] public string HorizonAction { get; set; } = null!;
    
    [FromForm(Name = "horizon-images")] public IFormFile[]? Horizon { get; set; }
    
    [FromForm(Name = "vertical")] public string VerticalAction { get; set; } = null!;
    
    [FromForm(Name = "vertical-images")] public IFormFile[]? Vertical { get; set; }
}