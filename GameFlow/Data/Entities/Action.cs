namespace GameFlow.Data;

public record Action
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}