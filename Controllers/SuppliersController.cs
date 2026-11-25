using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBusinessInventory.Data;

namespace SmallBusinessInventory.Controllers;

public class SuppliersController(InventoryDbContext db) : Controller
{
    public async Task<IActionResult> Index() =>
        View(await db.Suppliers.Include(s => s.Products).OrderBy(s => s.Name).ToListAsync());
}
