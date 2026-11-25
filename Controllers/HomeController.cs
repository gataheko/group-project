using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBusinessInventory.Data;

namespace SmallBusinessInventory.Controllers;

public class HomeController(InventoryDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        // Load active products into memory before calculating decimal totals.
        // SQLite cannot translate this decimal Sum expression in this query.
        var products = await db.Products
            .Where(p => !p.IsArchived)
            .ToListAsync();

        ViewBag.ProductCount = products.Count;
        ViewBag.LowStockCount = products.Count(p => p.Quantity <= p.ReorderLevel);
        ViewBag.InventoryValue = products.Sum(p => p.Price * p.Quantity);

        ViewBag.LowStock = await db.Products
            .Include(p => p.Category)
            .Where(p => !p.IsArchived && p.Quantity <= p.ReorderLevel)
            .OrderBy(p => p.Quantity)
            .Take(8)
            .ToListAsync();

        return View();
    }
}
