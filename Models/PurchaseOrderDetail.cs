using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SmallBusinessInventory.Models;
public class PurchaseOrderDetail
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    [Range(1, 1000000)] public int Quantity { get; set; }
    [Range(0.01, 1000000), Column(TypeName = "decimal(10,2)")] public decimal UnitPrice { get; set; }
}
