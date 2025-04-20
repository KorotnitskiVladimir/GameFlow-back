namespace GameFlow.Data;

public class UserData
{
    public Guid Id { get; set; }
    public string Login { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public string? AboutUser { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime RegDate { get; set; }
    // Добавить уже:
    // - список купленных продуктов
    // - ачивки?
    //
    // Можно добавить в диплом:
    // - настройки приватности
    // - инвентарь с ништяками

}