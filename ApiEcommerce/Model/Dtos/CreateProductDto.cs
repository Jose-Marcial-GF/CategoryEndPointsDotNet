using System;

namespace ApiEcommerce.Model.Dtos;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public IFormFile? Image { get; set; }
    public string SKU { get; set; } = string.Empty;

    public int Stock { get; set; }

    public DateTime CreationDate { get; set; }
    public DateTime? UpdateTime { get; set; } = null;

    public int CategoryId { get; set; }

}
