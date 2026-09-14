using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client.NativeInterop;

namespace ApiEcommerce.Model.Dtos;

public class CreateCategoryDto
{
    [Required(ErrorMessage ="Nombre Obligatorio")]    
    [MaxLength(50, ErrorMessage ="Muy largo, menos de 50 chars")]    
    [MinLength(3, ErrorMessage ="Al menos 50 chars")]    
    public string Name { get; set; } = String.Empty;


}
