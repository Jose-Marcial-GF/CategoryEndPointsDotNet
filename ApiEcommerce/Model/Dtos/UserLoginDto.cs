using System;
using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Model.Dtos;

public class UserLoginDto
{
    [Required(ErrorMessage ="El campo username está vacío")]
    
    public string? UserName { get; set; }
    [Required(ErrorMessage ="El campo password está vacío")]
    public string? Password { get; set; }
}
