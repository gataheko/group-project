using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBusinessInventory.Data;

// Allows controller to reference the Category class ~Rduehr
using SmallBusinessInventory.Models;

namespace SmallBusinessInventory.Controllers;

public class CategoriesController(InventoryDbContext db) : Controller {
    public async Task<IActionResult> Index() =>
        View(await db.Categories.Include(c => c.Products).OrderBy(c => c.Name).ToListAsync());

    public IActionResult Create() {
        return View(new Category());
    }

    // HttpPost makes the action only respond to HTTP post requests, ValidateAntiForgeryToken helps prevent cross site forgeries. ~rduehr
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description")] Category category) {
        category.Name = category.Name?.Trim() ?? "";
        
        if (!ModelState.IsValid)
            return View(category);

        // Make sure they don't try to duplicate a category that exists. ~Rduehr
        if (await db.Categories.AnyAsync(c => c.Name == category.Name)) {
            ModelState.AddModelError(nameof(Category.Name), "A category with this name already exists.");
            return View(category);
        }

        db.Categories.Add(category);
        await db.SaveChangesAsync();
        
        TempData["Message"] = "Category created successfully.";
        
        return RedirectToAction(nameof(Index));
    }
}
