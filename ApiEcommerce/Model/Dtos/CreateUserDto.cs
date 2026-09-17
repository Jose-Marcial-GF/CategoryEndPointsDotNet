using System;
using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Model.Dtos;

public class CreateUserDto
{

[Required(ErrorMessage ="El campo name está vacío")]
    public string? Name { get; set; }
[Required(ErrorMessage ="El campo username está vacío")]
    public string? UserName { get; set; }
[Required(ErrorMessage ="El campo password está vacío")]
    public string? Password { get; set; }
[Required(ErrorMessage ="El campo role está vacío")]
    public string? Role { get; set; }

}
