using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBusinessInventory.Data;

namespace SmallBusinessInventory.Controllers;

public class CategoriesController(InventoryDbContext db) : Controller
{
    public async Task<IActionResult> Index() =>
        View(await db.Categories.Include(c => c.Products).OrderBy(c => c.Name).ToListAsync());
}
