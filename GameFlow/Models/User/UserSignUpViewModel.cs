namespace GameFlow.Models.User;

public class UserSignUpViewModel
{
    public UserSignUpFormModel? FormModel { get; set; }
    public Dictionary<string, string>? ValidationErrors { get; set; }
}