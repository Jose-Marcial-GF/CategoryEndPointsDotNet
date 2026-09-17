using System;

namespace ApiEcommerce.Model.Dtos;

public class UserRegisterDto
{

    public string? ID  { get; set; }

    public required string? Name { get; set; }
    public required string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }
}
