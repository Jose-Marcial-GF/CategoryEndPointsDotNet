using System.ComponentModel.DataAnnotations;

public class Category
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = String.Empty;

    [Required]
    public DateTime CreatedAt  { get; set; }
    public DateTime CreationDate { get; internal set; }
}