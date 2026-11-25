using System.ComponentModel.DataAnnotations;
namespace SmallBusinessInventory.Models;
public class InventoryAdjustment
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public DateTime AdjustmentDate { get; set; } = DateTime.UtcNow;
    public int PreviousQuantity { get; set; }
    public int NewQuantity { get; set; }
    [Required, StringLength(250)] public string Reason { get; set; } = "";
}
