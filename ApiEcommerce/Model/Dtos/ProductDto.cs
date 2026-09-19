using System;

namespace ApiEcommerce.Model.Dtos;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Decimal Price { get; set; }

    public string? ImageUrl { get; set; }
    public string? ImageUrlLocal { get; set; }
    public string SKU { get; set; } = string.Empty;

    public int Stock { get; set; }

    public DateTime CreationDate { get; set; } = DateTime.Now;
    public DateTime? UpdateTime { get; set; } = null;

    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;



}
