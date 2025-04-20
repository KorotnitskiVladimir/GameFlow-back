﻿namespace GameFlow.Data;

public class UserAccess
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string RoleId { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string Salt { get; set; } = null!;
    public string Dk { get; set; } = null!;
    public UserData UserData { get; set; } = null!;
    public UserRole UserRole { get; set; } = null!;

}