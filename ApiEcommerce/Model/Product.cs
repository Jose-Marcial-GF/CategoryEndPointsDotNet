using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace ApiEcommerce.Model;

public class Product
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    [Column(TypeName ="decimal(18,2)")]
    public Decimal Price { get; set; }

    public string? ImageUrl { get; set; }
    public string? ImageUrlLocal { get; set; }
    [Required]
    public string SKU { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    public DateTime CreationDate { get; set; }
    public DateTime? UpdateTime { get; set; } = null;

    public int CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    public required Category Category { get; set; }
}
