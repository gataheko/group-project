using System.ComponentModel.DataAnnotations;

namespace SmallBusinessInventory.Models;

public class Category
{
    public int Id { get; set; }
    [Required, StringLength(80)]
    public string Name { get; set; } = "";
    [StringLength(250)]
    public string? Description { get; set; }
    public List<Product> Products { get; set; } = [];
}
