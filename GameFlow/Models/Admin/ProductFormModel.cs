using Microsoft.AspNetCore.Mvc;

namespace GameFlow.Models.Admin;

public class ProductFormModel
{
    [FromForm(Name = "category-id")]
    public Guid CategoryId { get; set; }
    
    [FromForm(Name = "product-name")]
    public string Name { get; set; } = null!;
    
    [FromForm(Name = "product-description")]
    public string Description { get; set; } = null!;
    
    [FromForm(Name = "product-slug")]
    public string? Slug { get; set; }
    
    [FromForm(Name = "product-image")]
    public IFormFile[] Images { get; set; } = null!;
    
    [FromForm(Name = "product-video")] 
    public IFormFile[]? Videos { get; set; }
    
    [FromForm(Name = "product-rating")]
    public int? Rating { get; set; }
    
    [FromForm(Name = "product-price")]
    public string Price { get; set; } = string.Empty;
    
    [FromForm(Name = "product-developer")]
    public string Developer { get; set; } = null!;
    
    [FromForm(Name = "product-publisher")]
    public string Publisher { get; set; } = null!;
    
    [FromForm(Name = "product-tags")]
    public string Tags { get; set; } = null!;
    
    [FromForm(Name = "product-mods")]
    public string SupportedMods { get; set; } = null!;
    
    [FromForm(Name = "product-platforms")]
    public string SupportedPlatforms { get; set; } = null!;
    
    [FromForm(Name = "product-releaseDate")]
    public DateTime ReleaseDate { get; set; }
}