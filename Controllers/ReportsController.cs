using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBusinessInventory.Data;

namespace SmallBusinessInventory.Controllers;

public class ReportsController(InventoryDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var products = await db.Products.Include(p => p.Category).Where(p => !p.IsArchived).ToListAsync();
        ViewBag.TotalValue = products.Sum(p => p.Price * p.Quantity);
        ViewBag.TotalUnits = products.Sum(p => p.Quantity);
        ViewBag.LowStockCount = products.Count(p => p.IsLowStock);
        ViewBag.CategoryTotals = products.GroupBy(p => p.Category?.Name ?? "Uncategorized")
            .Select(g => new CategoryTotal(g.Key, g.Sum(p => p.Quantity), g.Sum(p => p.Price * p.Quantity)))
            .OrderBy(x => x.Category).ToList();
        return View(products.OrderBy(p => p.Quantity).ToList());
    }
}
public record CategoryTotal(string Category, int Units, decimal Value);
