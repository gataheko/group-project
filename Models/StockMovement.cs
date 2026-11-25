using System.ComponentModel.DataAnnotations;
namespace SmallBusinessInventory.Models;
public class StockMovement
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public DateTime MovementDate { get; set; } = DateTime.UtcNow;
    public int QuantityChange { get; set; }
    [Required, StringLength(40)] public string MovementType { get; set; } = "Adjustment";
    [StringLength(250)] public string? Notes { get; set; }
}
