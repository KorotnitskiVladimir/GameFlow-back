using Microsoft.AspNetCore.Mvc;

namespace GameFlow.Models.User;

public class UserSignUpFormModel
{
    [FromForm(Name = "user-login")]
    public string UserLogin { get; set; } = null!;
    [FromForm(Name = "user-name")]
    public string UserName { get; set; } = null!;
    [FromForm(Name = "user-email")]
    public string UserEmail { get; set; } = null!;
    [FromForm(Name = "user-phone")]
    public string UserPhone { get; set; } = null!;
    [FromForm(Name = "user-password")]
    public string UserPassword { get; set; } = null!;
    [FromForm(Name = "repeat-password")]
    public string PasswordRepeat { get; set; } = null!;
    [FromForm(Name = "user-country")]
    public string Country { get; set; } = null!;
    [FromForm(Name = "user-birthDate")]
    public DateTime BirthDate { get; set; }
    public string? AvatarUrl { get; set; }
    public string? AboutUser { get; set; }
}