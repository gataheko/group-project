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

    // Edit action. Requires a valid ID and entry. ~rduehr
    public async Task<IActionResult> Edit(int id) {
        var category = await db.Categories.FindAsync(id);

        // Make sure the entity actually exists. ~rduehr
        if (category is null)
            return NotFound();
    
        return View(category);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Category input) {

        // ID validation to make sure it matches the one submitted by the form. ~rduehr
        if (id != input.Id)
            return BadRequest();
    
        input.Name = input.Name?.Trim() ?? "";
    
        if (!ModelState.IsValid)
            return View(input);
    
        var category = await db.Categories.FindAsync(id);
    
        if (category is null)
            return NotFound();

        // Made sure duplicate checking excludes the current category. ~rduehr
        if (await db.Categories.AnyAsync(c => c.Name == input.Name && c.Id != id)) {
            ModelState.AddModelError(nameof(Category.Name), "A category with this name already exists.");
    
            return View(input);
        }
    
        category.Name = input.Name;
        category.Description = input.Description;
    
        await db.SaveChangesAsync();
    
        TempData["Message"] = "Category updated successfully.";

        return RedirectToAction(nameof(Index));
    }
}
