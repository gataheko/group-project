using System.ComponentModel.DataAnnotations;
namespace SmallBusinessInventory.Models;
public class PurchaseOrder
{
    public int Id { get; set; }
    [Required, StringLength(30)] public string OrderNumber { get; set; } = "";
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    [Required, StringLength(30)] public string Status { get; set; } = "Draft";
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<PurchaseOrderDetail> Details { get; set; } = new List<PurchaseOrderDetail>();
}
