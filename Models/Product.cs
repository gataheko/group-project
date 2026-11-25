using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallBusinessInventory.Models;

public class Product
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    [Display(Name = "Product name")]
    public string Name { get; set; } = "";

    [Required, StringLength(40)]
    public string Sku { get; set; } = "";

    [StringLength(40)]
    public string? Barcode { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0.01, 1000000)]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [Range(0, 1000000)]
    public int Quantity { get; set; }

    [Range(0, 1000000)]
    [Display(Name = "Reorder level")]
    public int ReorderLevel { get; set; } = 5;

    [Range(1, int.MaxValue, ErrorMessage = "Select a category.")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Select a supplier.")]
    [Display(Name = "Supplier")]
    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public bool IsLowStock => Quantity <= ReorderLevel;
}
