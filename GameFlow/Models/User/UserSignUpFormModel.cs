namespace GameFlow.Models.User;

public class UserSignUpFormModel
{
    public string UserLogin { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public string UserPhone { get; set; } = null!;
    public string UserPassword { get; set; } = null!;
    public string PasswordRepeat { get; set; } = null!;
    public string Country { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public string? AvatarUrl { get; set; }
    public string? AboutUser { get; set; }
}