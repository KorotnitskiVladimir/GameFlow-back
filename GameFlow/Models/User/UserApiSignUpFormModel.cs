using Microsoft.AspNetCore.Mvc;

namespace GameFlow.Models.User;

public class UserApiSignUpFormModel
{
    [FromForm(Name = "login")]
    public string UserLogin { get; set; } = null!;
    [FromForm(Name = "name")]
    public string UserName { get; set; } = null!;
    [FromForm(Name = "email")]
    public string UserEmail { get; set; } = null!;
    [FromForm(Name = "phone")]
    public string UserPhone { get; set; } = null!;
    [FromForm(Name = "password")]
    public string UserPassword { get; set; } = null!;
    [FromForm(Name = "confirmPassword")]
    public string PasswordRepeat { get; set; } = null!;
    [FromForm(Name = "country")]
    public string Country { get; set; } = null!;
    [FromForm(Name = "birthDate")]
    public DateTime BirthDate { get; set; }
    public string? AvatarUrl { get; set; }
    public string? AboutUser { get; set; }
}