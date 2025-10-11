using System.Text.Json.Serialization;

namespace GameFlow.Data;

public record Product
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? ActionId { get; set; }
    public int? Rating { get; set; }  // Механизм обновления рейтинга???
    public int CopiesSold { get; set; } = 0; 
    public double Price { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? Slug { get; set; }
    public string ImagesCsv { get; set; } = null!;
    public string Developer { get; set; } = null!;
    public string Publisher { get; set; } = null!;
    public List<string> Tags { get; set; } = null!;
    public List<string> SupportedMods { get; set; } = null!;
    public List<string> SupportedPlatforms { get; set; } = null!;
    public DateTime ReleaseDate { get; set; }
    public DateTime? DeletedAt { get; set; }
    [JsonIgnore] public Category Category { get; set; } = null!;
    //[JsonIgnore] 
    public Action? Action { get; set; }
    // Reviews
    // SystemRequirements
    // Patches
    // Languages
    // Achievements
}