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
    // Добавить:
    // отдельный класс Library (ID + ссылка на UserId). в Library коллекция Library Items (Product, PurchasedAt,
    // TimePlayed, <Achievements>, <Reviews>
    // или сделать "инвентарь", в котором будет библиотека и всякие значки, картинки, фоны и т.п. Уровень пользователя
    // тоже можно привязать к инвентарю

}