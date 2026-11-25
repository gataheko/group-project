using System.ComponentModel.DataAnnotations;
namespace SmallBusinessInventory.Models;
public class Customer
{
    public int Id { get; set; }
    [Required, StringLength(120)] public string Name { get; set; } = "";
    [EmailAddress, StringLength(160)] public string? Email { get; set; }
    [Phone, StringLength(30)] public string? Phone { get; set; }
    [StringLength(250)] public string? Address { get; set; }
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}
