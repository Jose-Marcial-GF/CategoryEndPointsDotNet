using System;

namespace ApiEcommerce.Model.Dtos;

public class CategoryDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = String.Empty;

    public DateTime CreatedAt  { get; set; }

}
