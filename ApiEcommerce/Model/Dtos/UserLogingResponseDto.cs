using System;

namespace ApiEcommerce.Model.Dtos;

public class UserLogingResponseDto
{
    public UserDataDto? User { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }
}
