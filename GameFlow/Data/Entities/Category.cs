using System.Text.Json.Serialization;

namespace GameFlow.Data;

public record Category
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public DateTime? DeletedAt { get; set; }
    [JsonIgnore] public Category? ParentCategory { get; set; }
    
    public List<Product>? Products { get; set; }
}