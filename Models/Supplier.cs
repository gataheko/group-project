using System.ComponentModel.DataAnnotations;

namespace SmallBusinessInventory.Models;

public class Supplier
{
    public int Id { get; set; }
    [Required, StringLength(100)]
    public string Name { get; set; } = "";
    [StringLength(100)]
    public string? ContactName { get; set; }
    [EmailAddress, StringLength(150)]
    public string? Email { get; set; }
    [Phone, StringLength(30)]
    public string? Phone { get; set; }
    public List<Product> Products { get; set; } = [];
}
